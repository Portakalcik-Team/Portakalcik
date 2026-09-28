# 🗺️ Portakalcık - Harita Tasarımları

> **Versiyon:** v1.0 (Kavramsal Tasarımlar)  
> **Tarih:** 28 Eylül 2026  
> **Not:** Bu haritalar kavramsal taslaktır. Unity'de detaylandırılacak ve büyütülecektir (gerçek boyut: ~25x25 hücre).

---

## 📖 Harita Sembol Rehberi

```
█ = Duvar                    D = Kapı (canavar ölünce açılır)
. = Yol (yürünebilir alan)             → = Kısayol yönü
S = Giriş (Start)            
E = Çıkış (Exit)             

Canavarlar:                  Ağaçlar:
1 = 🐛 Diken Böceği          m = 🍊 Mandalina Ağacı
2 = 🦇 Gölge Yarasa           p = 🍊 Portakal Ağacı
3 = 🦊 Orman Tilkisi          l = 🍋 Limon Ağacı
4 = 🪨 Kök Golemi
5 = ⚔️ Labirent Muhafızı
```

### Tasarım İlkeleri
- **Tüm haritalar aynı boyutta** (13×13 grid = 6×6 hücre)
- Seviye arttıkça **daha fazla duvar, daha çok çıkmaz, daha karmaşık yollar**
- Canavarlar **kısayol kapılarını korur** — öldürünce kapı açılır, çıkışa daha hızlı gidilir
- Ana yol (uzun) her zaman açık; kısayollar canavara bağlı
- Ağaçlar **çıkmaz yollarda gizli** — keşfi ödüllendiriyor

---

## 🟢 Seviye 1 — "İlk Adım" (Kolay)

**Tema:** Basit U şekli, öğretici seviye  
**Zorluk:** ★☆☆☆☆

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S . . . . . . . . . . █
█ . █ █ █ █ █ █ █ █ █ . █
█ . █ . . . l . . . █ . █
█ . █ . █ █ █ █ █ . █ . █
█ . █ . m . 1 D . . . . █
█ . █ . █ █ █ █ █ █ █ . █
█ . . . l . . . . . █ . █
█ . █ █ █ █ █ █ █ . █ . █
█ . █ . . . 1 D . . █ . █
█ . █ . █ █ █ █ █ . █ . █
█ p . . . . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar | Konum Stratejisi |
|---|---|---|
| 🐛 Diken Böceği | 2 | Kısayol kapılarını koruyor |
| 🍊 Mandalina Ağacı | 1 | Orta kısımda, kolay bulunur |
| 🍊 Portakal Ağacı | 1 | Sol alt köşede |
| 🍋 Limon Ağacı | 2 | Çıkmaz yollarda |
| 🚪 Kısayol Kapısı | 2 | Canavar 1 ve 2'nin yanında |

**Kısayol mekaniği:**
- ⬆️ Canavar 1 öldürülünce → üst kısayol açılır → 4 hücre kısalır
- ⬇️ Canavar 2 öldürülünce → alt kısayol açılır → 6 hücre kısalır
- **Ana yol (canavarları öldürmeden):** Serpantin şeklinde, ~24 hücre

---

## 🟢 Seviye 2 — "Çatallanma" (Kolay)

**Tema:** İlk defa çoklu yol seçimi  
**Zorluk:** ★★☆☆☆

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S . . . . █ . . . l . █
█ . █ █ █ . █ . █ █ █ . █
█ . █ . l . . . █ . m . █
█ . █ . █ █ █ █ █ . █ . █
█ . 1 D . . . . █ . . . █
█ █ █ █ █ . █ . █ █ █ . █
█ . . . l . █ . █ . 1 D █
█ . █ █ █ . █ . █ . █ . █
█ . . . █ . 2 D . . █ . █
█ . █ . █ █ █ █ █ . █ . █
█ p . . . . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 2 |
| 🦇 Gölge Yarasa | 1 |
| 🍊 Mandalina Ağacı | 2 |
| 🍊 Portakal Ağacı | 1 |
| 🍋 Limon Ağacı | 3 |
| 🚪 Kısayol Kapısı | 3 |

**Kısayol mekaniği:**
- Sol Diken → Sol koridordan direk orta bölgeye geçiş
- Sağ Diken → Sağ koridordan alt kata hızlı iniş
- Yarasa → Hızlı ama tehlikeli kısayol (yarasa kaçabilir!)

---

## 🟡 Seviye 3 — "Labirent Başlıyor" (Orta)

**Tema:** İlk gerçek labirent hissi, yeni canavar: Tilki  
**Zorluk:** ★★☆☆☆

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S █ . l . █ . . . . . █
█ . █ . █ . █ . █ █ █ . █
█ . . . █ . m . . . █ . █
█ █ █ . █ █ █ █ █ . █ . █
█ 1 D . . . . . l . █ . █
█ . █ █ █ █ █ . █ █ █ . █
█ . . . █ . 3 D . . . . █
█ █ █ . █ . █ █ █ █ █ . █
█ . l . █ . █ . 2 D . . █
█ . █ . █ . █ . █ █ █ . █
█ p . . . . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 1 |
| 🦇 Gölge Yarasa | 1 |
| 🦊 Orman Tilkisi | 1 |
| 🍊 Mandalina Ağacı | 2 |
| 🍊 Portakal Ağacı | 1 |
| 🍋 Limon Ağacı | 4 |

**Yeni mekanik:** Orman Tilkisi kaçabilir! Iskaladığın atış = boşa mandalina.  
**Kısayollar:** 3 kapı, her biri farklı bölgeyi birbirine bağlıyor.

---

## 🟡 Seviye 4 — "Artı Kesişim" (Orta)

**Tema:** Merkezi kavşak, 4 yöne dallanma  
**Zorluk:** ★★★☆☆

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S █ . l . █ . l . █ . █
█ . █ . █ . █ . █ . █ . █
█ . 1 D . . █ . . . █ . █
█ █ █ █ █ . █ . █ █ █ . █
█ l . . m . . . 1 D . . █
█ . █ █ █ . █ . █ █ █ █ █
█ . l . . . █ . . . l . █
█ █ █ █ █ . █ █ █ . █ . █
█ . . . █ . █ . . 3 D . █
█ . █ . █ . █ █ █ █ █ . █
█ p . . l . . . l . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 2 |
| 🦊 Orman Tilkisi | 1 |
| 🦇 Gölge Yarasa | 1 |
| 🍊 Mandalina Ağacı | 2 |
| 🍊 Portakal Ağacı | 1 |
| 🍋 Limon Ağacı | 4 |

**Tasarım:** Merkezdeki kavşakta 4 yöne gidilebilir. Hangi yolun çıkışa gittiğini bulmak zor — limonlar çözerek ipucu al!

---

## 🟠 Seviye 5 — "Spiral" (Zor)

**Tema:** Dıştan içe spiral, yeni canavar: Kök Golemi  
**Zorluk:** ★★★☆☆

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S . . l . █ . l . . . █
█ . █ █ █ . █ . █ █ █ . █
█ . █ . l . . . █ . m . █
█ . █ . █ █ █ █ █ █ █ . █
█ . 1 D . . 4 D . . █ . █
█ . █ █ █ █ █ █ █ █ █ . █
█ . . . . . 3 D . . l . █
█ . █ . █ █ █ █ █ █ █ . █
█ . █ . l . █ . . 1 D . █
█ . █ █ █ . █ . █ █ █ . █
█ p . . l . . . l . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 1 |
| 🦇 Gölge Yarasa | 1 |
| 🦊 Orman Tilkisi | 1 |
| 🪨 Kök Golemi | 1 |
| 🍊 Mandalina Ağacı | 3 |
| 🍊 Portakal Ağacı | 1 |
| 🍋 Limon Ağacı | 5 |

**Yeni tehlike:** Kök Golemi! Yavaş ama 10+ hasar. Kısayolunu açmak çok mandalina gerektirir ama BÜYÜK kısayol sağlar.  
**Spiral:** Dıştan başla, spiralleyerek merkeze ulaş, merkezden çıkışa kısayol.

---

## 🟠 Seviye 6 — "Izgara Tuzak" (Zor)

**Tema:** Grid benzeri yapı, çok sayıda çıkmaz  
**Zorluk:** ★★★★☆

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S █ . l . █ . █ . l . █
█ . █ D █ . █ . █ . █ . █
█ . 2 . . . 1 D . . █ . █
█ █ █ . █ █ █ . █ █ █ . █
█ . l . █ . 1 D . . m . █
█ . █ . █ █ █ . █ █ █ . █
█ . 2 D . . l . █ . 1 . █
█ . █ █ █ █ █ . █ D █ . █
█ . . . l . . . █ . . . █
█ . █ . █ █ █ . █ . █ . █
█ p . . . . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 2 |
| 🦇 Gölge Yarasa | 2 |
| 🦊 Orman Tilkisi | 1 |
| 🍊 Mandalina Ağacı | 3 |
| 🍊 Portakal Ağacı | 1 |
| 🍋 Limon Ağacı | 5 |

**Tasarım:** Izgaraya benzer yapı — her yöne gidilebilir gibi görünür ama çoğu çıkmaz! Dikkatli harita kullanımı şart.

---

## 🔴 Seviye 7 — "Zigzag Kabusu" (Çok Zor)

**Tema:** Dar zigzag koridorlar, pusu noktaları  
**Zorluk:** ★★★★☆

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S █ . █ . █ l █ . █ . █
█ . █ . █ . █ . █ . █ . █
█ . . . 1 D . . 2 D . . █
█ █ █ . █ . █ █ █ . █ █ █
█ . m . █ . l . █ . 3 D █
█ . █ █ █ . █ █ █ █ █ . █
█ . 2 D . . █ . l . 1 . █
█ . █ █ █ █ █ . █ █ █ . █
█ . l . █ . . . █ . . . █
█ . █ . █ D █ . █ . █ . █
█ p . . l . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 1 |
| 🦇 Gölge Yarasa | 2 |
| 🦊 Orman Tilkisi | 1 |
| 🪨 Kök Golemi | 1 |
| 🍊 Mandalina Ağacı | 3 |
| 🍊 Portakal Ağacı | 2 |
| 🍋 Limon Ağacı | 5 |

**Tehlike:** Dar koridorlarda canavarlara denk gelmek kaçınılmaz. Zigzag yapı sürekli yön değiştirir — mini harita hayati önem taşır!

---

## 🔴 Seviye 8 — "Dallanma Ağacı" (Çok Zor)

**Tema:** Ağaç gibi dallanma, her dal farklı risk/ödül  
**Zorluk:** ★★★★★

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S █ . █ . . . █ . l . █
█ . █ D █ . █ . █ . █ . █
█ . 1 . . . 2 D . . m . █
█ █ █ . █ █ █ . █ █ █ . █
█ . l . █ . 3 D . . █ . █
█ . █ . █ █ █ █ █ █ █ . █
█ . 2 D . . █ . l . 1 . █
█ . █ █ █ █ █ . █ D █ . █
█ . . . l . 3 . . . █ . █
█ . █ . █ █ █ . █ . █ . █
█ p . . l . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 1 |
| 🦇 Gölge Yarasa | 2 |
| 🦊 Orman Tilkisi | 2 |
| 🪨 Kök Golemi | 1 |
| 🍊 Mandalina Ağacı | 4 |
| 🍊 Portakal Ağacı | 2 |
| 🍋 Limon Ağacı | 6 |

**Strateji:** Her dalda farklı canavarlar bekliyor. Sol dal: kolay canavarlar ama uzun yol. Sağ dal: zor canavarlar ama kısa yol. Hangisini seçeceksin?

---

## 🔴 Seviye 9 — "Muhafız Kalesi" (Çok Zor)

**Tema:** Labirent Muhafızı ilk kez çıkıyor! İç içe geçmiş yapı  
**Zorluk:** ★★★★★

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S █ . █ . █ . l . █ . █
█ . █ D █ . █ D █ . █ . █
█ . 1 . . . 2 . . . l . █
█ █ █ . █ █ █ . █ █ █ . █
█ . m . █ . 3 D . . █ . █
█ . █ . █ █ █ █ █ █ █ . █
█ . 2 D . . █ . 5 . l . █
█ . █ █ █ █ █ . █ D █ . █
█ . . . l . 1 . . . █ . █
█ . █ . █ █ █ . █ . █ . █
█ p . . l . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 1 |
| 🦇 Gölge Yarasa | 2 |
| 🦊 Orman Tilkisi | 1 |
| 🪨 Kök Golemi | 1 |
| ⚔️ **Labirent Muhafızı** | **1** |
| 🍊 Mandalina Ağacı | 4 |
| 🍊 Portakal Ağacı | 2 |
| 🍋 Limon Ağacı | 6 |

**BOSS MEKANİĞİ:** Labirent Muhafızı çıkışa en yakın kısayolu koruyor! 48 HP (16 mandalina vuruşu). Öldürmezsen uzun yoldan git. Öldürürsen çıkışa neredeyse direkt ulaş!

---

## ⚫ Seviye 10 — "Son Labirent" (EFSANE)

**Tema:** En karmaşık yapı, tüm canavarlar, final!  
**Zorluk:** ★★★★★

```text
█ █ █ █ █ █ █ █ █ █ █ █ █
█ S █ . █ l █ . █ . █ . █
█ . █ D █ D █ . █ D █ . █
█ . 1 . 2 . . . 1 . l . █
█ █ █ . █ . █ . █ █ █ . █
█ . m . █ 3 D . █ . 2 . █
█ . █ █ █ . █ █ █ █ █ . █
█ . 4 D . . █ l 5 . . . █
█ . █ █ █ █ █ . █ D █ . █
█ . l . . . █ . . . █ . █
█ . █ . █ █ █ D █ . █ . █
█ p . . l . l . . . . . E
█ █ █ █ █ █ █ █ █ █ █ █ █
```

| Öğe | Miktar |
|---|---|
| 🐛 Diken Böceği | 1 |
| 🦇 Gölge Yarasa | 2 |
| 🦊 Orman Tilkisi | 2 |
| 🪨 Kök Golemi | 1 |
| ⚔️ **Labirent Muhafızı** | **1** |
| 🍊 Mandalina Ağacı | 5 |
| 🍊 Portakal Ağacı | 2 |
| 🍋 Limon Ağacı | 7 |

**FİNAL TASARIMI:**
- En fazla çıkmaz yol (10+)
- En uzun ana yol (30+ hücre)
- 7 kısayol kapısı — hepsi canavarla korunuyor
- Kök Golemi merkezde devasa kısayolu koruyor (36 HP = 12 vuruş!)
- Labirent Muhafızı çıkış öncesi son engel (51 HP = 17 vuruş!)
- Mandalina ekonomisi çok sıkı — her vuruş önemli!

---

## 📊 Harita Karşılaştırma Tablosu

| Sv. | Tema | Canavar | Ağaç | Kısayol | Çıkmaz | Zorluk |
|---|---|---|---|---|---|---|
| 1 | U-Şekli | 2 | 5 | 2 | 2 | ★☆☆☆☆ |
| 2 | Çatallanma | 3 | 6 | 3 | 3 | ★★☆☆☆ |
| 3 | İlk Labirent | 3 | 7 | 3 | 4 | ★★☆☆☆ |
| 4 | Artı Kesişim | 4 | 7 | 3 | 5 | ★★★☆☆ |
| 5 | Spiral | 4 | 9 | 4 | 6 | ★★★☆☆ |
| 6 | Izgara Tuzak | 5 | 9 | 5 | 7 | ★★★★☆ |
| 7 | Zigzag | 5 | 10 | 5 | 8 | ★★★★☆ |
| 8 | Dal Ağacı | 6 | 12 | 6 | 9 | ★★★★★ |
| 9 | Muhafız Kalesi | 6 | 12 | 6 | 10 | ★★★★★ |
| 10 | Son Labirent | 7 | 14 | 7 | 12 | ★★★★★ |

---

## 🎯 Kısayol Stratejisi Özeti

```
Kısayol Değeri (zaman tasarrufu)
  ▲
  │  ⚔️ Muhafız        ← En büyük kısayol, en zor canavar
  │       🪨 Golem      
  │           🦊 Tilki  
  │               🦇 Yarasa
  │                   🐛 Diken  ← En küçük kısayol, en kolay canavar
  └──────────────────────────────► Zorluk
```

**Temel kural:** Daha zor canavarlar = Daha değerli kısayollar

- 🐛 **Diken** kısayolu: 3-5 hücre kısaltır (2-5 mandalina)
- 🦇 **Yarasa** kısayolu: 5-7 hücre kısaltır (3-6 mandalina)
- 🦊 **Tilki** kısayolu: 7-10 hücre kısaltır (4-10 mandalina)
- 🪨 **Golem** kısayolu: 10-14 hücre kısaltır (6-12 mandalina)
- ⚔️ **Muhafız** kısayolu: 14-20 hücre kısaltır (8-17 mandalina)

---

## 🔧 Unity'de Büyütme Notları

Bu 13×13 kavramsal tasarımlar Unity'de **~25×25 hücre** boyutuna büyütülecek:

| Özellik | Kavramsal (şimdi) | Unity (gerçek) |
|---|---|---|
| Grid boyutu | 13×13 | ~51×51 |
| Hücre sayısı | 36 | ~625 |
| Koridor genişliği | 1 birim | 3-4 birim (oyuncu sığmalı) |
| Duvar yüksekliği | — | 3 birim (first-person perspektif) |
| Tavan | — | Kapalı (karanlık atmosfer) |
| Işık | — | Oyuncu etrafında sınırlı ışık |

> ⚠️ Bu tasarımlar **kavramsal taslaktır**. Asıl labirentler Unity'de birlikte detaylandırılacak, test edilecek ve dengelenecektir.

---

> 📝 *Harita tasarımları denge testleri sonrası güncellenecektir.*
