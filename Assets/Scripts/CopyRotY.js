#pragma strict

public var myTarget : Transform;

function Start () {

}

function Update () {
	transform.rotation.eulerAngles.y = myTarget.rotation.eulerAngles.y;
}