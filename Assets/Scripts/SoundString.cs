using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundString : MonoBehaviour
{
    public class SegmentData {
        public Vector2 posNew;
        public Vector2 posOld;

        public SegmentData(Vector2 pos) {
            this.posNew = pos;
            this.posOld = pos;
        }
    }

    public class ContactData {
        public float side; // + for right side, - for left side
        public Transform transform;
    
        public ContactData(float s, Transform t) {
            side = s;
            transform = t;
        }
    }

    public Vector2 startPos { get { return transform.position + transform.up * coll.size.y / 2; }}
    public Vector2 endPos { get { return transform.position - transform.up * coll.size.y / 2; }}

    BoxCollider2D coll;
    LineRenderer lineRenderer;

    AudioSource audioSource;
    [Range(0, 5)] public int note;
    public AudioClip[] pluckClips;
    public GameObject pingRing;

    private List<SegmentData> stringSegs;
    private float length;
    public int numSegs;
    public int reps;
    public float segLength { get { return length / numSegs; }}

    public Dictionary<Collider2D, ContactData> contactors;
    public Triggerable[] targets;

    private void Start() {
        coll = GetComponent<BoxCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();

        stringSegs = new List<SegmentData>();
        contactors = new Dictionary<Collider2D, ContactData>();

        length = (endPos - startPos).magnitude;
        lineRenderer.positionCount = numSegs;

        for (int i = 0; i < numSegs; i++) {
            Vector2 pos = Vector2.Lerp(startPos, endPos, (float)i / (float)(numSegs - 1));
            stringSegs.Add(new SegmentData(pos));
        }
    }

    private void OnDisable() {
        contactors = new Dictionary<Collider2D, ContactData>();
    }

    private void FixedUpdate() {
        // determine if the string is being pulled by anyone
        int contactIndexPos = 0;
        
        contactors = contactors.Where((pair) => pair.Value.transform != null).ToDictionary(p => p.Key, p => p.Value);
        ContactData contactData = contactors.Values.FirstOrDefault(contact => {
            float side = Vector2.Dot(transform.right, contact.transform.position - transform.position);
            return side != 0 && Mathf.Sign(contact.side) != Mathf.Sign(side);
        });

        if (contactData != null) {
            // which string segment should snap to the contact point?
            Vector2 diff = endPos - startPos;
            float ratio = Vector2.Dot(diff.normalized, contactData.transform.position.xy() - startPos) / diff.magnitude;
            contactIndexPos = (int)Mathf.Clamp(numSegs * ratio, 1, numSegs - 3);
        }

        // apply verlet integration
        for (int i = 0; i < numSegs; i++) {
            SegmentData seg = stringSegs[i];
            Vector2 velocity = seg.posNew - seg.posOld;
            seg.posOld = seg.posNew;
            seg.posNew += velocity;
        }

        // apply constraints
        for (int a = 0; a < reps; a++) {
            stringSegs[0].posNew = startPos;
            stringSegs[numSegs - 1].posNew = endPos;

            for (int i = 0; i < numSegs - 1; i++) {
                // keep ends of string constant
                SegmentData firstSeg = stringSegs[i];
                SegmentData secondSeg = stringSegs[i + 1];

                // enforce string length
                Vector2 diff = secondSeg.posNew - firstSeg.posNew;
                Vector2 correction = diff.normalized * (diff.magnitude - segLength);
                if (i == 0) {
                    stringSegs[i + 1].posNew -= correction;
                }
                else if (i == numSegs - 2) {
                    stringSegs[i].posNew += correction;
                }
                else {
                    stringSegs[i].posNew += correction / 2;
                    stringSegs[i + 1].posNew -= correction / 2;
                }

                // keep contact point constant
                if (contactData != null && contactIndexPos == i) {
                    stringSegs[i].posNew = contactData.transform.position;
                    stringSegs[i + 1].posNew = contactData.transform.position;
                }
            }
        }
    }

    private void Update() {
        // draw line
        Vector3[] segPositions = stringSegs.Select(seg => seg.posNew.xyz()).ToArray();
        lineRenderer.SetPositions(segPositions);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (contactors.ContainsKey(other)) {
            return;
        }

        float side = Vector2.Dot(transform.right, other.transform.position - transform.position);
        ContactData contactData = new ContactData(side, other.transform);
        contactors.Add(other, contactData);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (!contactors.ContainsKey(other)) {
            return;
        }

        float side = Vector2.Dot(transform.right, other.transform.position - transform.position);
        if (Mathf.Sign(side) != Mathf.Sign(contactors[other].side)) {
            // string was plucked
            audioSource.PlayOneShot(pluckClips[note]);
            Instantiate(pingRing, other.transform.position, Quaternion.identity)
                .GetComponent<PingRing>().Init(1.5f, 3f, 0.1f, Color.white.WithA(0.5f));

            foreach (Triggerable target in targets)
                target.Trigger(this);
        }
        contactors.Remove(other);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        coll = GetComponent<BoxCollider2D>();
        if (coll != null)
            Gizmos.DrawLine(startPos, endPos);
    }
}
