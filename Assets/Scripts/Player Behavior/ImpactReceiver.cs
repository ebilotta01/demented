using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;

public class ImpactReceiver: MonoBehaviour {
  
  public float mass = 3.0f; // define the character mass
  Vector3 impact = Vector3.zero;
  [SerializeField]
  CharacterController character;
  
  void Start(){
    character = GetComponent<CharacterController>();
    }
 
  public void AddImpact(Vector3 force){ // CharacterController version of AddForce
    impact += force / mass;
    }
 
   void Update(){ 
     if (impact.magnitude > 0.2f){
       character.Move(impact * Time.deltaTime);
       }
       impact = Vector3.Lerp(impact, Vector3.zero, 5.0f*Time.deltaTime);
       }
      }