﻿using UnityEngine;
using System.Collections;

public class Kamikaze : Mob {

	private int speed = 110, followDistance = 10;
	private Vector3 playerPosition;
	
	public Sprite[] spriteWalk;
	private int walkingFrame;
	private int idleFrame;
	
	private GameObject body;
	
	public override string getName ()
	{
		return "Kamikaze";
	}
	
	public override Vector3 getTargetLocation ()
	{
		return playerPosition;
	}
	
	void create (){
		body = new GameObject ("PlayerHead");
		body.transform.SetParent (transform);
		body.transform.position = transform.position;
		body.AddComponent<SpriteRenderer>();
		body.GetComponent<SpriteRenderer>().sortingOrder = 2;
		stats.strength = -50;
	}
	
	// Use this for initialization
	public override void OnStart () {
		create ();
		transform.rotation = new Quaternion(0,0,0,0);
		spriteWalk = Resources.LoadAll<Sprite>("Sprite/demon");
		replaceSkill(0, new SkillSelfDestruct (this));
	}
	
	// Update is called once per frame
	public override void OnUpdate () 
	{
		if (GameObject.FindWithTag ("Player") && Mathf.Sqrt(Mathf.Pow(playerPosition.x - transform.position.x, 2) + Mathf.Pow(playerPosition.y - transform.position.y, 2)) <= 1.5)
			skills[0].useSkill();
	}

	public override void OnDeath() {
		skills[0].useSkill ();
	}

	public override Quaternion rotation {
		get {
			return body.transform.rotation;
		}
		set {
			body.transform.rotation = value;
		}
	}
	
	public override Transform feetTransform {
		get {
			return body.transform;
		}
	}
	
	private float timer;
	public override void movement () {
		if (GameObject.FindWithTag ("Player")) {
			GameObject player = GameObject.FindWithTag ("Player");
			playerPosition = player.transform.position;
			Vector3 diff = (player.transform.position - position).normalized;
			float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
			rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
			if (Mathf.Sqrt(Mathf.Pow(playerPosition.x - position.x, 2) + Mathf.Pow(playerPosition.y - position.y, 2)) <= followDistance) {
				GetComponent<Rigidbody2D>().AddForce(feetTransform.up * speed);
			} else
				GetComponent<Rigidbody2D>().AddForce(feetTransform.up * speed/4);
		}
		if (gameObject.GetComponent<Rigidbody2D>().velocity.magnitude > 0.05f) {
			if (timer + (1.20 - Mathf.Pow(gameObject.GetComponent<Rigidbody2D>().velocity.magnitude, 0.1f)) <= Time.fixedTime) {
				if(walkingFrame >= spriteWalk.Length)
					walkingFrame = 0;
				body.GetComponent<SpriteRenderer> ().sprite = spriteWalk[walkingFrame];
				walkingFrame++;
				timer = Time.fixedTime;
			}
		}
	}
}