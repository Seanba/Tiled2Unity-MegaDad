using UnityEngine;
using System.Collections;

// The Octupus Battery "AI" is pretty simple, it just moves along a line until it collides with terrain and then changes direction.
public class OctopusBatteryAI : MonoBehaviour
{
    // seanba - testing with this (test ignore case and enum value that doesn't match)
    public enum MyEnumType
    {
        EnumValue1,
        EnumValue2,
        EnumValue3,
    }

    // Inspector properties
    // seanba - just testing for now
    public string MyString = "some value";
    public int MyInt = 1337;
    public float MyFloat = Mathf.PI;
    public bool MyBool = true;
    public MyEnumType MyEnum = MyEnumType.EnumValue1;

    private Vector2 direction = new Vector2(0, 1.0f);
    private float speed = 10.0f;

    private void Update()
    {
        BoxCollider2D box = this.gameObject.GetComponent<BoxCollider2D>();
        Vector2 box_w = this.gameObject.transform.position;
        box_w += box.offset;

        float dt = this.speed * Time.deltaTime;
        Vector2 dv = dt * this.direction;
        RaycastHit2D hit = Physics2D.BoxCast(box_w, box.size, 0, this.direction, dt, 1);
        if (hit.collider != null)
        {
            dv *= hit.fraction;
            //Debug.LogFormat("seanba - dv = {0}", dv);
        }

        this.gameObject.transform.Translate(dv);

        //box.offset;
        //box.size;
    }

}
