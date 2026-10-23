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

## Kim ne yaptı

| Üye | Bu harita |
|---|---|
| Hasan Sivaslıoğlu | Map_0: graybox, scriptler, fizik, hareket |
