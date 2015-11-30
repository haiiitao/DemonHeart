using UnityEngine;
using System.Collections;

public class SkillPowershot : Skill
{
	int maxDistance = 20;

	public SkillPowershot(Mob mob) : base(mob) { }

	public override string getName () {
		return "Powershot";
	}
	
	public override Sprite getImage () {
		return Resources.Load<Sprite>("Skills/Powershot/powershot");
	}
	
	public override float getAttackSpeed () {
		return 1.5f;
	}
	
	public override float getMaxCooldown () {
		return 1.5f * (1 - mob.stats.cooldownReduction / 100);
	}
	
	public override float getManaCost () {
		return 20;
	}
	
	public override void skillLogic() {
		Vector2 targetLocation = mob.headTransform.up * maxDistance + mob.headTransform.position;
		Vector2 startLocation = mob.headTransform.position + mob.headTransform.up;
		foreach (RaycastHit2D linecast in Physics2D.LinecastAll(startLocation, targetLocation)) {
			if (linecast.collider.CompareTag("Wall")) {
				targetLocation = linecast.point;
				break;
			}
		}
		GameObject powershot = new GameObject();
		powershot.transform.position = new Vector2(startLocation.x + (targetLocation.x - startLocation.x)/2, startLocation.y + (targetLocation.y - startLocation.y)/2);
		powershot.transform.rotation = mob.headTransform.rotation;
		BoxCollider2D collider = powershot.AddComponent<BoxCollider2D>();
		collider.isTrigger = true;
		collider.transform.localScale = new Vector2(0.5f, Vector2.Distance(startLocation, targetLocation));
		PowershotEffect p = powershot.AddComponent<PowershotEffect>();
		p.mob = mob;
		LineRenderer lineRenderer = powershot.AddComponent<LineRenderer>();
		lineRenderer.material = Resources.Load<Material>("Skills/Powershot/PowershotLaser");
		lineRenderer.sortingOrder = 4;
		lineRenderer.SetPosition(0, startLocation);
		lineRenderer.SetPosition(1, targetLocation);
		AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Skills/Powershot/sniperShot"), mob.headTransform.position);
	}
}

class PowershotEffect : MonoBehaviour {
	
	float alpha = 0.8f;
	Color c = Color.white;
	string enemyTag;
	public Mob mob;

	void Start() {
		if (mob.gameObject.tag == "Player" || mob.gameObject.tag == "Ally")
			enemyTag = "Enemy"; 
		else
			enemyTag = "Player";
	}

	void FixedUpdate () {
		c.a = alpha;
		gameObject.GetComponent<LineRenderer>().SetColors(c, c);
		alpha -= 0.03f;
		if (alpha <= 0)
			Destroy(gameObject);
		else if (alpha <= 0.5f)
			gameObject.GetComponent<BoxCollider2D>().enabled = false;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.CompareTag(enemyTag))
			collider.GetComponent<Mob>().hurt ((2 * mob.stats.basicAttackDamage) + (0.2f * mob.stats.attackDamage));
	}
}