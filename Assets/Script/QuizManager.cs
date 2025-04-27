using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class QuizManager : MonoBehaviour
{
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
        public Sprite spriteButton;
        public AudioClip suaraButton; // aku tambahin suara button tolong buatkan caranya gimana agar suaranya berdasarkan nama warna aja karena ada difficult sulit yg spritenya tidak sesuai teks jadi tolong suara button nya buat sama dengan teks nya
    }
    [Header("Data Warna")]
    public List<WarnaData> semuaWarna;

    [Header("Variasi Soal")]
    public List<VariasiSoal> variasiSoalList;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;

    public Image imageLingkaran, pointImage;
    public List<Button> tombolJawaban;
    public List<TextMeshProUGUI> teksJawaban;
    public Button tombolSubmit;

    public Sprite basicButtonSprite;
    public TextMeshProUGUI teksPertanyaan, nilai;


    private HashSet<Sprite> usedLingkaranSprites = new HashSet<Sprite>();

    private string jawabanBenar;
    private int indexDipilih = -1;
    private int indexSoal = 0;
    private int skor = 0;
    private string level;
    private List<WarnaData> soalSaatIni;

    [Header("Image Jawaban")]
    public List<Sprite> gambarJawabanBenar;
    public List<Sprite> gambarJawabanSalah;
    public Image imageJawaban;

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

    // Fallback jika semua sprite sudah dipakai
    if (soal == null)
    {
        usedLingkaranSprites.Clear(); // Reset
        soal = soalSaatIni[0];
        usedLingkaranSprites.Add(soal.spriteLingkaran);
    }

    imageLingkaran.sprite = soal.spriteLingkaran;
    jawabanBenar = soal.namaWarna;


    // 2. Random pertanyaan
    // Pilih variasi soal secara acak
    VariasiSoal variasiDipilih = variasiSoalList[Random.Range(0, variasiSoalList.Count)];
    teksPertanyaan.text = variasiDipilih.teksPertanyaan;
    StartCoroutine(PutarAudioSoalDenganDelay(variasiDipilih.audioPertanyaan, 0.95f));


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
    private IEnumerator PutarAudioSoalDenganDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }


    public void PilihJawaban(int index)
    {
        indexDipilih = index;

        for (int i = 0; i < tombolJawaban.Count; i++)
        {
            tombolJawaban[i].GetComponent<Image>().color = (i == index) ? Color.yellow : Color.white;
        }
        string warnaDipilih = teksJawaban[index].text;
        PutarSuaraWarna(warnaDipilih);
    }

    private IEnumerator TampilkanGambarJawabanBenar()
    {
        int randomIndex = Random.Range(0, gambarJawabanBenar.Count);
        imageJawaban.sprite = gambarJawabanBenar[randomIndex];
        imageJawaban.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        imageJawaban.gameObject.SetActive(false);
        indexSoal++;
        TampilkanSoal();
    }


    private IEnumerator TampilkanGambarJawabanSalah()
    {
        int randomIndex = Random.Range(0, gambarJawabanSalah.Count);
        imageJawaban.sprite = gambarJawabanSalah[randomIndex];
        imageJawaban.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(2);
        
        imageJawaban.gameObject.SetActive(false); 
        indexSoal++; 
        TampilkanSoal(); 
    }

    public void SubmitJawaban()
    {
        if (indexDipilih == -1)
        {
            Debug.Log("Belum memilih jawaban");
            return;
        }

        string jawabanPemain = teksJawaban[indexDipilih].text;
        bool benar = jawabanPemain == jawabanBenar;

        // Cek apakah ini soal terakhir
        bool soalTerakhir = indexSoal >= 4;

        if (soalTerakhir)
        {
            if (benar) skor += 20;

            // Langsung tampilkan skor tanpa coroutine
            Debug.Log("Selesai! Skor akhir: " + skor);
            nilai.text = skor.ToString();
            pointImage.gameObject.SetActive(true);

            // Nonaktifkan tombol-tombol jawaban
            foreach (var tombol in tombolJawaban)
            {
                tombol.interactable = false;
            }
            tombolSubmit.interactable = false;
        }
        else
        {
            if (benar)
            {
                skor += 20;
                Debug.Log("Benar! Skor: " + skor);
                StartCoroutine(TampilkanGambarJawabanBenar());
                audioSource.PlayOneShot(suaraBenar);
            }
            else
            {
                Debug.Log("Salah! Skor: " + skor);
                StartCoroutine(TampilkanGambarJawabanSalah());
                audioSource.PlayOneShot(suaraSalah);
            }
        }
    }
    public void PutarSuaraWarna(string namaWarna)
    {
        WarnaData data = semuaWarna.Find(w => w.namaWarna == namaWarna);
        if (data != null && data.suaraButton != null)
        {
            audioSource.PlayOneShot(data.suaraButton);
        }
        else
        {
            Debug.LogWarning("Suara tidak ditemukan untuk warna: " + namaWarna);
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

