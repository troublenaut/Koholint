#pragma strict

var speedX : float = 0;
var speedY : float = 0;
var speedZ : float = 0;

function Start () {

}

function Update () {

	transform.Rotate(Vector3(1,0,0),speedX*Time.deltaTime);
	transform.Rotate(Vector3(0,1,0),speedY*Time.deltaTime);
	transform.Rotate(Vector3(0,0,1),speedZ*Time.deltaTime);

}