#pragma strict

//https://www.youtube.com/watch?v=AmwfMpf499U

//Variables
var mirror : GameObject;
var character : GameObject;
var offset : float;
var diectionFaced : Direction;

function Update () {

	if( diectionFaced == Direction.X){
		offset =  (mirror.transform.position.x - character.transform.position.x);
		
		transform.position.x = mirror.transform.position.x + offset;
		transform.position.y = character.transform.position.y;
		transform.position.z = character.transform.position.z;
	}
	
	if( diectionFaced == Direction.Y){
		offset =  (mirror.transform.position.y - character.transform.position.y);
		
		transform.position.x = character.transform.position.x;
		transform.position.y = mirror.transform.position.y + offset;
		transform.position.z = character.transform.position.z;
	}
	
	if( diectionFaced == Direction.Z){
		offset =  (mirror.transform.position.y - character.transform.position.y);
		
		transform.position.x = character.transform.position.x;
		transform.position.y = character.transform.position.y;
		transform.position.z = mirror.transform.position.z + offset;
	}

}

public enum Direction{
	X,Y,Z
}