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
