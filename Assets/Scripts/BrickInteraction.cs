using UnityEngine;

public class Block : MonoBehaviour
{
    public GameObject breakParticles; // prefab delle particelle

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) // colpo da sotto
            {
                Instantiate(breakParticles, transform.position, Quaternion.identity);
                Destroy(gameObject); // distruggi il blocco
            }
        }
    }

}
