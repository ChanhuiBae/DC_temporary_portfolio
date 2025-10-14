using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[ExcelAsset]
public class DC : ScriptableObject
{
	public List<Entity_Character> character;
	public List<Entity_Monster> Monster;
	public List<Entity_Squad> Squad;
	public List<Entity_SquadMember> SquadMember;
	public List<Entity_Item> Item;
	public List<Entity_Artifact> Artifact;
	public List<Entity_AritifactCombination> ArtifactCombination;
	public List<Entity_Skill> Skill;
	public List<Entity_Script> Script;
}
