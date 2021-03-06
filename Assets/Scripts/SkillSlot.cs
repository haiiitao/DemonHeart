﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillSlot : MonoBehaviour {
	
	public int index;
	Slider slider;
	Image image;
	Text text;
	Skill skill;

	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider>();
		image = GetComponentInChildren<Image>();
		text = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        GameObject player = GameObject.Find("Player");
        if (player != null && player.GetComponent<Mob>().skills[index] != null) {
			skill = player.GetComponent<Mob>().skills[index];
			slider.value = skill.remainingCooldown()/(skill.properties["cooldown"] * (1 - (player.GetComponent<Mob>().stats.cooldownReduction / 100)) + 0.001f);
			image.sprite = skill.getImage();
			if (skill.remainingCooldown() > 0)
				text.text = ((int)skill.remainingCooldown() + 1).ToString ();
			else
				text.text = "";
		}
	}
}
