﻿using UnityEngine;
using System.Collections;

public class SkillChainLightning : Skill {

	int timesCanChain = 2;
	int maxDistance = 10;

	public SkillChainLightning() : base() {
        addBaseProperty("chainCount", timesCanChain);
    }

	public override string getName () {
		return "Chain Lightning";
	}
	
	public override Sprite getImage () {
		return Resources.Load<Sprite>("Skills/ChainLightning/chainLightningIcon");
	}
	
	public override float getMaxCooldown () {
		return 0.2f;
	}
	
	public override float getManaCost () {
		return 5;
	}

	public override void skillLogic (Entity mob, Stats stats)
	{
//        Debug.Log("chainlightning");
		GameObject chainLightning = GameObject.Instantiate(Resources.Load<GameObject>("Skills/ChainLightning/Chainlightning"));
		chainLightning.transform.position = mob.headTransform.position;
		chainLightning.GetComponent<ChainLightning>().stats = stats;
		chainLightning.GetComponent<ChainLightning>().chainTimes = (int)properties["chainCount"];
		chainLightning.GetComponent<ChainLightning>().maxDistance = maxDistance;
		Vector3 targetLocation;
		if (Vector3.Distance(mob.headTransform.position, mob.getTargetLocation()) > maxDistance)
			targetLocation = mob.headTransform.position + (mob.headTransform.up * maxDistance);
		else
			targetLocation = mob.getTargetLocation();
		foreach (RaycastHit2D lineCast in Physics2D.LinecastAll(mob.headTransform.position, targetLocation)) {
			if (lineCast.collider.CompareTag("Wall")) {
				targetLocation = lineCast.point;
				break;
			}
		}
		GameObject enemy = FindClosestEnemy(mob, stats.tag, targetLocation);
		if (enemy != null) {
			chainLightning.GetComponent<ChainLightning>().enemy = enemy;
			chainLightning.GetComponent<ChainLightning>().target = enemy.GetComponent<Mob>().feetTransform.position;
		} else {
			chainLightning.GetComponent<ChainLightning>().target = targetLocation;
		}
	}

	GameObject FindClosestEnemy(Entity mob, string tag, Vector3 target) {
		string enemyTag;
		if (tag == "Player" || tag == "Ally")
			enemyTag = "Enemy"; 
		else
			enemyTag = "Player";
		GameObject[] gos = GameObject.FindGameObjectsWithTag(enemyTag);
		ArrayList inRange = new ArrayList();
		foreach(GameObject go in gos) {
			if (Vector3.Distance(go.transform.position, mob.headTransform.position) < maxDistance)
				inRange.Add(go);
		}
		GameObject closest = null;
		float distance = Mathf.Infinity;
		foreach (var go in inRange) {
			float curDistance = Vector3.Distance(((GameObject)go).transform.position, target);
			if (curDistance < distance) {
				bool add = true;
				foreach (RaycastHit2D lineCast in Physics2D.LinecastAll(mob.headTransform.position, ((GameObject)go).transform.position)) {
					if (lineCast.collider.CompareTag("Wall")) {
						add = false;
						break;
					}
				}
				if (add) {
					closest = (GameObject)go;
					distance = curDistance;
				}
			}
		}
		return closest;
	}
}
