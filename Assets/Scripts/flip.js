#pragma strict

function Start () {
	InvokeRepeating("JustKeepFlipping", 0, 0.4);
}

function Update () {

}

function JustKeepFlipping () {
	transform.localScale.x *= -1;
}