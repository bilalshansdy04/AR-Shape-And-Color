using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class WarnaData
    {
        public string namaWarna;
        public Sprite spriteLingkaran;
        public Sprite spriteButton;
    }


    public Image imageLingkaran, pointImage;
    public List<Button> tombolJawaban;
    public List<TextMeshProUGUI> teksJawaban;
    public Button tombolSubmit;

    [Header("Data Warna")]
    public List<WarnaData> semuaWarna;
    public Sprite basicButtonSprite; // Untuk level sedang
    public TextMeshProUGUI teksPertanyaan, nilai;


    private string jawabanBenar;
    private int indexDipilih = -1;
    private int indexSoal = 0;
    private int skor = 0;
    private string level;
    private List<WarnaData> soalSaatIni;

    void Start()
    {
        level = DifficultyManager.tingkatKesulitanDipilih;
        TampilkanSoal();
    }

    void TampilkanSoal()
{
    indexDipilih = -1;

    // 1. Acak soal & tampilkan pertanyaan
    soalSaatIni = new List<WarnaData>(semuaWarna);
    soalSaatIni.Shuffle();
    WarnaData soal = soalSaatIni[0];
    imageLingkaran.sprite = soal.spriteLingkaran;
    jawabanBenar = soal.namaWarna;

    // 2. Random pertanyaan
    string[] variasiSoal = { "Warna apakah ini?", "Coba tebak warna ini?", "Apa nama warna ini?" };
    string pertanyaanDipilih = variasiSoal[Random.Range(0, variasiSoal.Length)];
    // Asumsikan ada TMP Text di UI bernama teksPertanyaan
    teksPertanyaan.text = pertanyaanDipilih;

    // 3. Siapkan pilihan
    List<WarnaData> pilihan = new List<WarnaData> { soal };
    for (int i = 1; i < semuaWarna.Count && pilihan.Count < 4; i++)
    {
        if (!pilihan.Exists(w => w.namaWarna == soalSaatIni[i].namaWarna))
        {
            pilihan.Add(soalSaatIni[i]);
        }
    }
    pilihan.Shuffle();

    // 4. Tampilkan tombol & sprite sesuai level
    List<WarnaData> spriteAcakUnik = new List<WarnaData>(semuaWarna);
    spriteAcakUnik.RemoveAll(w => pilihan.Exists(p => p.namaWarna == w.namaWarna));
    spriteAcakUnik.Shuffle();

    for (int i = 0; i < tombolJawaban.Count; i++)
    {
        teksJawaban[i].text = pilihan[i].namaWarna;

        switch (level)
        {
            case "Mudah":
                tombolJawaban[i].image.sprite = pilihan[i].spriteButton;
                break;

            case "Sedang":
                tombolJawaban[i].image.sprite = basicButtonSprite;
                break;

            case "Sulit":
                // Buat daftar untuk menyimpan sprite yang sudah digunakan
                HashSet<Sprite> usedSprites = new HashSet<Sprite>();

                for (int j = 0; j < tombolJawaban.Count; j++)
                {
                    // Set sprite untuk tombol saat ini
                    if (j < pilihan.Count)
                    {
                        // Pilih sprite yang berbeda dari jawaban benar
                        WarnaData acak;
                        do
                        {
                            acak = semuaWarna[Random.Range(0, semuaWarna.Count)];
                        } while (acak.namaWarna == jawabanBenar || usedSprites.Contains(acak.spriteButton));

                        tombolJawaban[j].image.sprite = acak.spriteButton;
                        usedSprites.Add(acak.spriteButton);
                    }

                    teksJawaban[j].text = pilihan[j].namaWarna;
                }
                break;
        }

        tombolJawaban[i].GetComponent<Image>().color = Color.white;
    }
}

    public void PilihJawaban(int index)
    {
        indexDipilih = index;

        for (int i = 0; i < tombolJawaban.Count; i++)
        {
            tombolJawaban[i].GetComponent<Image>().color = (i == index) ? Color.yellow : Color.white;
        }
    }

    public void SubmitJawaban()
    {
        if (indexDipilih == -1)
        {
            Debug.Log("Belum memilih jawaban");
            return;
        }

        string jawabanPemain = teksJawaban[indexDipilih].text;
        if (jawabanPemain == jawabanBenar)
        {
            skor += 20;
            Debug.Log("Benar! Skor: " + skor);
        }
        else
        {
            Debug.Log("Salah! Skor: " + skor);
        }

        indexSoal++;
        if (indexSoal >= 5)
        {
            Debug.Log("Selesai! Skor akhir: " + skor);
            pointImage.gameObject.SetActive(true);
            nilai.text = skor.ToString();
        }
        else
        {
            TampilkanSoal();
        }
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

