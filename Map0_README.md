# Map_0 — "İlk Hasat"

- **Sahne:** `Assets/Scenes/Map_0.unity`
- **Hazırlayan:** Hasan Sivaslıoğlu (branch: `Hasan`)
- **Oyun tipi:** A — Taşı ve Teslim Et
- **Plan:** `Portakalcik_Harita_Tasarimlari.md` içindeki "Seviye 0 — İlk Hasat" yılan labirenti (13×13 grid, her hücre 3 m, toplam 39 × 39 m).

## Çekirdek döngü

Oyuncu sol üstteki girişten (S) başlar, sarı portakalları (kargo) bulur, onu başının üstünde taşıyarak labirentin sağındaki yeşil çıkışa (E) götürür. Kırmızı alanlar kargoyu düşürür. Sondaki çatalda bir seçim var: **kısa yol**, hareketli kırmızı bloğun gidip geldiği koridor (zamanlama gerekir); **uzun yol**, alttan dolaşan, tehlikesiz ama buzlu rampası olan koridor. Rampanın üstündeki platformda üçüncü portakal durur.

## Renk kodu

| Renk | Anlamı | Haritada |
|---|---|---|
| Gri | geçilmez | duvarlar (M_Architecture) |
| Açık gri | yürünebilir | zemin, platform, rampa (M_Traversable) |
| Sarı | alınabilir | portakallar / kargo (M_Interactive) |
| Yeşil | buraya getir | çıkış / teslim alanı (M_Goal) |
| Kırmızı | tehlike | kırmızı şeritler ve hareketli blok (M_Hazard) |

## Fiiller (Verbs)

| Fiil | Karşılaştığı üç şey | Hafta |
|---|---|---|
| yürü | dar koridorlar, kırmızı şeritler, buzlu rampa | 4 |
| zıpla | kırmızı şeritler, platform, hareketli blok | 4 |
| al | yerdeki portakal, platformdaki portakal, düşen portakal | 3 |
| bırak | kırmızı alan (zorla), E tuşu (isteyerek), tekrar alma beklemesi | 3–4 |
| teslim et | yeşil çıkış, kısa yol, uzun yol | 3 |

## Feel notları

| Ayar | Değer | Neden |
|---|---|---|
| Max Speed | 5 m/s | 39 m'lik labirentte bir koridor (33 m) yaklaşık 7 saniyede geçiliyor; 3 m'lik koridorlarda duvara çarpmadan dönülebiliyor. |
| Acceleration | 40 | Tam hıza 0.14 saniyede çıkıyor: tepkili ama robotik değil. Hareketli bloğun yanından geçerken anında durup kalkabiliyorum. |
| Turn Speed | 720°/s | Koridor dönüşlerinde burun hemen yeni yöne bakıyor; 90 deneyince burun geride kalıyordu. |
| Jump Force | 4 | Yaklaşık 0.8 m yükselme: 1.5 m'lik kırmızı şeritlerin üstünden atlanıyor; 1 m'lik platforma sadece koşarak çıkılıyor, bu yüzden rampa önemini koruyor. |
| Buz tutunması (grip) | 0.04 | Buzda hız değişimi 40 × 0.04 = 1.6 m/s². Rampa 11° olduğu için yerçekimi bunu yeniyor: yavaş giden geri kayıyor, hızlı ve düz giden platforma çıkıyor. 9.5°'de bu iki değer neredeyse eşit, oyuncu rampada kalıyor, o yüzden hocanın 11°'si seçildi. |

## Kim ne yaptı

| Üye | Bu harita |
|---|---|
| Hasan Sivaslıoğlu | Map_0: graybox, scriptler, fizik, hareket |

## Eğitim katmanı (Map0_Egitim)

Map_0 artık oyunun eğitim haritası. Oyuncu yılan yolunda ilerlerken 13 ipucu alanından geçiyor. Her alan ekranın altında bir kart açıyor, sağ üstteki sayaç kaç tanesinin görüldüğünü gösteriyor.

| Adım | Yer | Öğrettiği |
|---|---|---|
| 1 | Başlangıç | WASD, Space (sadece yerdeyken) |
| 2 | 1. portakal | Al, taşı, E ile bırak |
| 3 | Kırmızı şeritler | Tehlike portakalı düşürür |
| 4–9 | Canavar Galerisi | 5 canavar: hız, can, hasar, kaç mandalina gerektiği |
| 10 | Canlı Diken Böceği | Gerçek canavardan kaçma (değerse portakal düşer) |
| 11 | Yol ayrımı | Kısa yol mu, uzun yol mu |
| 12 | Buzlu rampa | Buzda tutunma %4 |
| 13 | Çıkış | Teslim et |

**Kurulum:** Map_0'ı aç → üst menü **Map0 → Eğitimi Kur** → kaydet. Tekrar çalıştırılabilir: eski `Map0_Egitim` nesnesini silip baştan kurar. 1–4. haftanın nesnelerine dokunmaz.

**Mimari:**
- `Map0MonsterData` (ScriptableObject): canavarın sayıları ve metni veri dosyasında, koddan ayrı. Değerler `Portakalcik_Denge_Sistemi.md`'den, Seviye 1 değerleri. Dosyalar: `Assets/Project/Prefabs/Map0/Egitim/`.
- `Map0TutorialHint`: görünmez trigger. Player girince ilerlemeye ve panele haber verir.
- `Map0TutorialProgress`: hangi ipuçlarının görüldüğünü `HashSet` ile sayar, değişince `event` ile duyurur (panel bunu dinler).
- `Map0HintPanel`: kartı doldurur, açılıp kapanmayı yumuşatır. Komşu alana geçerken eski alanın çıkışı yeni kartı kapatmasın diye "şu anki ipucu" kontrolü yapar.
- `Map0TutorialBuilder` + `Map0TutorialVisuals` (Editor): sahneyi koddan kurar. Canavar modelleri hazır şekillerden yapılır, malzemeleri `Art/Materials/Map0/Egitim/` altında.
- Canlı Diken Böceği W02/W03 scriptlerini tekrar kullanıyor: `Map0Patroller` (hız 2 m/s = oyuncu 5 × 0.4) + `Map0Hazard` + kinematic Rigidbody.

## 5. hafta: Kamera (Cinemachine)

Sahne: `Map0_Week5_CM_Follow` (takip), `Map0_Week5_CM_Ramp` (buzlu tırmanış, başta kapalı), `Map0_Week5_RampaBolgesi` (`Map0CameraZone`). Main Camera'da Cinemachine Brain var, kamerayı artık o yönetiyor. Kurulum: **Map0 → Hafta 5 Kamerayı Kur**, sonra kaydet. Son olarak CM_Follow'da **Save During Play**'i elle işaretle (bu ayar her bilgisayarda ayrı).

Kargoyu düşüren her tehlikede bir `CinemachineImpulseSource` var: Week 3 tehlike prefab'ı, hareketli blok ve eğitimdeki canlı Diken Böceği. `Map0Hazard` kargoyu düşürünce `GenerateImpulse()` çağırıyor. E ile bırakınca sarsıntı yok.

### Kamera feel notları

| Ayar | Değer | Neden |
|---|---|---|
| Camera Distance | 14 m | 3 m'lik koridorda önündeki iki dönemeci görüyorsun. Labirentin tamamı (39 m) görünmüyor, keşif hissi kalıyor. |
| Damping | 1, 1, 1 | 0 her adımda görüntüyü sarsar. 3'te oyuncu 5 m/s ile ekranın kenarına kaçar. 1 sakin ama kontrol hâlâ sende. |
| Dead Zone | 0.1 × 0.1, derinlik 1 | Kırmızı şeritlerin üstünden zıplamak kamerayı oynatmıyor. Yürüyünce takip başlıyor. |
| Lookahead | 0.4 s, smoothing 10, Ignore Y | 5 × 0.4 = 2 m ileri bakış, koridorun sonundaki tehlike erken görünüyor. Ignore Y açık, zıplama kamerayı yukarı itmiyor. |
| Sarsıntı | 0.5 aşağı, 0.2 s, Bump | Sadece "portakalı kaybettin" anında. Küçük tutuldu, oyuncuyu rahatsız etmesin. |
| Rampa kamerası | 60°, 16 m, geçiş 1 s | Yukarıdan bakınca rampanın eğimi ve üstteki platform birlikte görünüyor. Rampa kısa (5 m), 2 s'lik geçiş bitmeden tırmanış başlar; 1 s yetiyor. |

## 6. hafta: Kargo pedleri, hafif ve ağır kargo, skor

Elle konan üç portakal (`Map0_Week3_Kargo_01–03`) yerine aynı yerlerde üç ped var: `Map0_Week6_KargoPad_01–03`. Kırmızı şeritlerin arasında bir ağır ped var: `Map0_Week6_KargoPad_Agir`. Pedler `Map0CargoSpawner` ile, pedde kargo yoksa süre dolunca yeni kargo yapıyor. Kurulum: **Map0 → Hafta 6 Kargo Pedlerini Kur**, sonra kaydet.

- **Kaynak:** pedler. **Çıkış (sink):** yeşil çıkış, kargoyu siliyor ve puan veriyor (`Map0DeliveryZone`, Console'da `Teslim edildi! +3   Skor: 4`).
- **Veri dosyaları:** `Assets/Project/Data/Map0/CT_Map0_Hafif.asset`, `CT_Map0_Agir.asset` (`Map0CargoType`, ScriptableObject).
- **Prefab varyantı:** `Map0_Kargo_Agir`, `Map0_Kargo`'nun varyantı. Turuncu (`M_Map0_Agir`, E07B1A), daha büyük (1 m), daha yavaş ve alçak süzülüyor (0.15 m, hız 1). Sadece renge güvenmiyoruz, renk körü oyuncu da büyüklükten ayırt edebilsin.
- **Ağırlığın bedeli:** ağır kargo oyuncunun kütlesini 1'den 2'ye çıkarıyor (`Map0PlayerCarry`). Zıplama aynı itme (4) olduğu için hız yarıya iniyor (4 m/s → 2 m/s) ve oyuncu 0.8 m yerine yaklaşık 0.2 m yükseliyor. Kırmızı şeritlerin üstünden atlanamıyor. Yürüme hızı değişmiyor, çünkü `Map0PlayerMove` hızı doğrudan ayarlıyor.

### Balance notes

| Ayar | Hafif | Ağır | Neden |
|---|---|---|---|
| Puan | 1 | 3 | Ağırı almak bir risk, karşılığı 3 kat. |
| Ağırlık | 0 | 1 kg | Zıplamayı elinden alıyor. |
| Geri gelme süresi | 4 s | 8 s | Ağır ped seyrek dolsun, her seferinde "şimdi mi alsam?" diye düşünülsün. |
| Ped sayısı | 3 | 1 | Seçenek var ama arena kargoyla dolmuyor. |
| Yer | başlangıç köşesi, yolun döndüğü köşe, rampa platformu | 3. satırda iki kırmızı şeridin arası | Ağır kargo zorunlu yolun en başında. Alırsan bütün labirent boyunca zıplayamadan taşıyorsun, kısa yolda da hareketli bloğun içinden geçiyorsun. |

**Test oyuncusu (2 dakika):**

| Hafif teslim | Ağır teslim | "Neden onu aldın?" | Değiştirdiğim tek sayı |
|---|---|---|---|
| | | | |

## Commit notu

W01–W04 commitleri **geç** gönderildi.
