﻿using UnityEngine;
using System.Collections;

public class SkillExplosiveArrow : Skill {
	
	public Projectile projectile;
    SkillType projectileSkill = new ProjectileSkill();
    AoeSkill aoeSkill = new AoeSkill();

    public SkillExplosiveArrow() : base() {
        addSkillType(projectileSkill);
        addSkillType(aoeSkill);
    }
	
	public override string getName () {
		return "Explosive Arrow";
	}
	
	public override Sprite getImage () {
		return Resources.Load<Sprite>("Skills/ExplosiveArrow/explosiveArrowIcon");
	}
	
	public override float getAttackSpeed () {
		return 1f;
	}
	
	public override float getMaxCooldown () {
		return 0.5f;
	}
	
	public override float getManaCost () {
		return 25;
	}
	
	public override void skillLogic (Entity mob, Stats stats) {
        attack(mob, stats);
		AudioSource.PlayClipAtPoint(Resources.Load<AudioClip>("Skills/pew"), mob.headTransform.position);
    }

    void attack(Entity mob, Stats stats) {
        float y = Vector3.Distance(mob.getTargetLocation(), mob.headTransform.position);
        if (y > 6)
            y = 6;
        else if (y < 2)
            y = 2;
        float angleOfSpread = (((1 - (y - 2) / 4) * 3) + 1) * 5;
        for (int i = 0; i < properties["projectileCount"]; i++) {
            fireArrow(mob, stats, ((properties["projectileCount"] - 1) * angleOfSpread / -2) + i * angleOfSpread);
        }
    }

    void fireArrow(Entity mob, Stats stats, float rotate = 0) {
		//Instantiates the projectile with some speed
		GameObject basicArrow = MonoBehaviour.Instantiate (Resources.Load ("Skills/Arrow_Placeholder")) as GameObject;
        GameObject arrowGlow = MonoBehaviour.Instantiate(Resources.Load("ShotGlow")) as GameObject;
        arrowGlow.transform.position = new Vector3(basicArrow.transform.position.x, basicArrow.transform.position.y, -0.3f);
        arrowGlow.transform.SetParent(basicArrow.transform);
        projectile = new ExplosiveArrowProjectile (basicArrow, stats, this);
		basicArrow.GetComponent<basic_projectile> ().setProjectile (projectile);
		//Initiates the projectile's position and rotation
		basicArrow.transform.position = mob.headTransform.position;
		basicArrow.transform.rotation = mob.headTransform.rotation;
		basicArrow.transform.Translate (Vector3.up * 0.7f);
		basicArrow.transform.RotateAround (basicArrow.transform.position, Vector3.forward, rotate);
		projectile.projectileOnStart();
        projectile.chainTimes = properties["chainCount"];
    }
}

class ExplosiveArrowProjectile : Projectile {
	public ExplosiveArrowProjectile(GameObject gameObject, Stats stats, Skill skill) : base(gameObject, stats, skill) {}
	public override void OnHit () {
        RaycastHit2D[] hit = Physics2D.LinecastAll(gameObject.transform.position - gameObject.transform.up * 0.47f, gameObject.transform.position + gameObject.transform.up * 2f);
        Vector3 target = gameObject.transform.position;
        foreach (RaycastHit2D x in hit) {
            if (x.collider.CompareTag(collider.tag)) {
                target = x.point;
                break;
            }
        }
        GameObject explosion = GameObject.Instantiate(Resources.Load<GameObject>("Skills/ExplosiveArrow/FireExplosion"));
		explosion.GetComponent<ExplosiveArrowExplosion>().damage = 2 * stats.attackDamage;
		explosion.transform.position = target;
        explosion.transform.localScale = new Vector3(explosion.transform.localScale.x * skill.properties["areaOfEffect"], explosion.transform.localScale.y * skill.properties["areaOfEffect"], explosion.transform.localScale.z);
        explosion.transform.RotateAround(explosion.transform.position, Vector3.forward, Random.Range(0, 360));
		explosion.GetComponent<ExplosiveArrowExplosion>().enemyTag = Mob.getEnemyTag(stats.tag); 
	}
	public override float getSpeed () {
		return 40;
	}
	public override float getDuration () {
		return 0.5f;
	}
	public override float getDamage () {
		return 1 * stats.basicAttackDamage;
	}
}

