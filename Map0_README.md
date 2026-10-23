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

## Kim ne yaptı

| Üye | Bu harita |
|---|---|
| Hasan Sivaslıoğlu | Map_0: graybox, scriptler, fizik, hareket |
