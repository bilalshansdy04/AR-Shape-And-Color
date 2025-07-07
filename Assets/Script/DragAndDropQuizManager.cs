using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DragAndDropQuizManager : MonoBehaviour
{
    // --- Data Internal (Sama seperti script lama Anda) ---
    [System.Serializable]
    public class VariasiSoal
    {
        public string teksPertanyaan;
        public AudioClip audioPertanyaan;
    }

    [System.Serializable]
    public class WarnaData
    {
        public string namaWarna;
        public Sprite spriteLingkaran;
        public Sprite spriteButton; // Sprite untuk item yang di-drag
        public AudioClip suaraButton;
    }

    [Header("Data Soal & Warna")]
    public List<WarnaData> semuaWarna;
    public List<VariasiSoal> variasiSoalList;

    [Header("Komponen UI")]
    public Image imageLingkaranSoal;
    public TextMeshProUGUI teksPertanyaan;
    public DropZone dropZone; // Referensi ke area drop
    public List<DragItem> pilihanJawabanItems; // Daftar item jawaban yang bisa di-drag
    public List<Transform> posisiAwalJawaban; // Posisi awal untuk setiap item jawaban
    public List<TextMeshProUGUI> teksJawaban; // Teks di atas item jawaban

    [Header("Feedback & Skor")]
    public AudioSource audioSource;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;
    public Image imageJawaban; // Gambar feedback benar/salah
    public List<Sprite> gambarJawabanBenar;
    public List<Sprite> gambarJawabanSalah;
    public TextMeshProUGUI nilai;
    public Image pointImage;
    public Sprite basicButtonSprite;


    // --- Variabel Logika Game ---
    private string jawabanBenar;
    private int indexSoal = 0;
    private int skor = 0;
    private string level;
    private bool sudahSubmit = false;
    private List<WarnaData> soalSaatIni;
    private HashSet<Sprite> usedLingkaranSprites = new HashSet<Sprite>();

    void Start()
    {
        // Pastikan DropZone terhubung ke script ini
        if (dropZone != null)
        {
            // Memberi tahu DropZone bahwa script manager ini adalah KuisGeserManajer
            dropZone.SetQuizManager(this);
        }

        // Ambil level dari DifficultyManager (jika ada)
        // (Pastikan Anda punya script DifficultyManager di scene)
        if (FindObjectOfType<DifficultyManager>() != null) {
            level = DifficultyManager.tingkatKesulitanDipilih;
        } else {
            level = "Mudah"; // Default jika tidak ada DifficultyManager
        }

        TampilkanSoal();
    }

    void TampilkanSoal()
    {
        // 1. Reset Posisi dan state
        sudahSubmit = false;
        for (int i = 0; i < pilihanJawabanItems.Count; i++)
        {
            pilihanJawabanItems[i].transform.position = posisiAwalJawaban[i].position;
            pilihanJawabanItems[i].gameObject.SetActive(true);
        }
        if (dropZone != null) dropZone.GetComponent<Image>().raycastTarget = true;

        // 2. Acak soal & tampilkan pertanyaan
        soalSaatIni = new List<WarnaData>(semuaWarna);
        soalSaatIni.Shuffle();

        WarnaData soal = null;
        foreach (var warna in soalSaatIni)
        {
            if (!usedLingkaranSprites.Contains(warna.spriteLingkaran))
            {
                soal = warna;
                usedLingkaranSprites.Add(warna.spriteLingkaran);
                break;
            }
        }
        if (soal == null) // Jika semua sudah dipakai, reset
        {
            usedLingkaranSprites.Clear();
            soal = soalSaatIni[0];
            usedLingkaranSprites.Add(soal.spriteLingkaran);
        }

        imageLingkaranSoal.sprite = soal.spriteLingkaran;
        jawabanBenar = soal.namaWarna;

        // 3. Random pertanyaan
        VariasiSoal variasiDipilih = variasiSoalList[Random.Range(0, variasiSoalList.Count)];
        teksPertanyaan.text = variasiDipilih.teksPertanyaan;
        StartCoroutine(PutarAudioSoalDenganDelay(variasiDipilih.audioPertanyaan, 0.5f));

        // 4. Siapkan dan tampilkan pilihan jawaban pada item drag
        List<WarnaData> pilihan = new List<WarnaData> { soal };
        for (int i = 1; i < semuaWarna.Count && pilihan.Count < pilihanJawabanItems.Count; i++)
        {
            if (!pilihan.Exists(w => w.namaWarna == soalSaatIni[i].namaWarna))
            {
                pilihan.Add(soalSaatIni[i]);
            }
        }
        pilihan.Shuffle();

        for (int i = 0; i < pilihanJawabanItems.Count; i++)
        {
            pilihanJawabanItems[i].namaWarna = pilihan[i].namaWarna;
            teksJawaban[i].text = pilihan[i].namaWarna;
            pilihanJawabanItems[i].suaraItem = pilihan[i].suaraButton;

            Image itemImage = pilihanJawabanItems[i].GetComponent<Image>();
            switch (level)
            {
                case "Mudah":
                    itemImage.sprite = pilihan[i].spriteButton;
                    break;
                case "Sedang":
                    itemImage.sprite = basicButtonSprite;
                    break;

                case "Sulit":
                    // Logika untuk memastikan sprite acak dan tidak duplikat
                    WarnaData dataSpriteAcak;
                    do {
                        dataSpriteAcak = semuaWarna[Random.Range(0, semuaWarna.Count)];
                    } while (dataSpriteAcak.namaWarna == pilihan[i].namaWarna); // Pastikan tidak sama dengan warna teks
                    itemImage.sprite = dataSpriteAcak.spriteButton;
                    break;
            }
        }
    }

    public void CekJawaban(DragItem itemYangDiDrop)
    {
        if (sudahSubmit) return;
        sudahSubmit = true;

        foreach (var item in pilihanJawabanItems)
        {
            if (item != itemYangDiDrop)
            {
                item.gameObject.SetActive(false);
            }
        }
        if (dropZone != null) dropZone.GetComponent<Image>().raycastTarget = false;

        bool benar = itemYangDiDrop.namaWarna == jawabanBenar;
        bool soalTerakhir = indexSoal >= 4; // Asumsi ada 5 soal (index 0-4)

        if (soalTerakhir)
        {
            if (benar) skor += 20;
            nilai.text = skor.ToString();
            pointImage.gameObject.SetActive(true);
            audioSource.PlayOneShot(benar ? suaraBenar : suaraSalah);
        }
        else
        {
            if (benar)
            {
                skor += 20;
                StartCoroutine(TampilkanGambarJawabanBenar());
                audioSource.PlayOneShot(suaraBenar);
            }
            else
            {
                StartCoroutine(TampilkanGambarJawabanSalah(itemYangDiDrop));
                audioSource.PlayOneShot(suaraSalah);
            }
        }
    }

    private IEnumerator PutarAudioSoalDenganDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (clip != null) audioSource.PlayOneShot(clip);
    }

    private IEnumerator TampilkanGambarJawabanBenar()
    {
        imageJawaban.sprite = gambarJawabanBenar[Random.Range(0, gambarJawabanBenar.Count)];
        imageJawaban.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        imageJawaban.gameObject.SetActive(false);
        indexSoal++;
        TampilkanSoal();
    }

    private IEnumerator TampilkanGambarJawabanSalah(DragItem itemSalah)
    {
        int itemIndex = pilihanJawabanItems.IndexOf(itemSalah);
        itemSalah.transform.position = posisiAwalJawaban[itemIndex].position;
        
        imageJawaban.sprite = gambarJawabanSalah[Random.Range(0, gambarJawabanSalah.Count)];
        imageJawaban.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        imageJawaban.gameObject.SetActive(false);
        indexSoal++;
        TampilkanSoal();
    }
}