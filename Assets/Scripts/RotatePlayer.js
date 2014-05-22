#pragma strict
 
var speed = 75.0;
 
function Update() {
	if (Input.GetButton("BButton") == false) {
   		var v3 = Vector3(0.0, Input.GetAxis("Horizontal"), 0.0);
   		transform.Rotate(v3 * speed * Time.deltaTime);
   	}
}