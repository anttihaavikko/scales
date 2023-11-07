using System;
using System.Collections.Generic;
using UnityEngine;

public class NoteTrack : MonoBehaviour
{
    [SerializeField] private Note notePrefab;

    private readonly List<Note> notes = new();
    private int position = -1;

    private void Start()
    {
        for (var i = 0; i < 12; i++)
        {
            var note = Instantiate(notePrefab, transform);
            note.transform.position = new Vector3(-5f + i, 0);
            notes.Add(note);
            note.gameObject.SetActive(false);
        }
    }

    public void Add(int number, bool sharp, bool flat)
    {
        position = (position + 1) % notes.Count;
        notes[position].gameObject.SetActive(true);
        notes[position].Show(number, sharp, flat);
    }
}