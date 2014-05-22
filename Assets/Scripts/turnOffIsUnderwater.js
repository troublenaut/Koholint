#pragma strict


function Start () {

}

function Update () {

	if (transform.position.y < 0) {
		GetComponent(MeshRenderer).enabled = false;
	} else {
		GetComponent(MeshRenderer).enabled = true;
	}

}