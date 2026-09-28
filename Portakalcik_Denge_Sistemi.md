# 🍊 Portakalcık - Oyun Denge Sistemi Önerisi

> **Versiyon:** v1.0 (Taslak — birlikte düzenlenecek)  
> **Tarih:** 28 Eylül 2026

---

## 🎯 Temel Değerler

| Parametre | Değer |
|---|---|
| Oyuncu Canı | **100 HP** |
| Mandalina Hasarı | **3 HP/vuruş** |
| Toplam Seviye | **10** |
| Canavar Türü | **5** |

---

## 👾 Canavar Sistemi

### Canavar Türleri & Özellikleri

| # | Canavar | Hız (Oyuncu=1.0) | Saldırı Hızı | Özellik |
|---|---|---|---|---|
| 1 | 🐛 **Diken Böceği** | 0.4 (çok yavaş) | 2.0s | Kolay hedef, düşük hasar |
| 2 | 🦇 **Gölge Yarasa** | 1.2 (oyuncudan hızlı!) | 1.0s | Uçar, hızlı ama kırılgan |
| 3 | 🦊 **Orman Tilkisi** | 0.9 (neredeyse eşit) | 1.5s | Dengeli, kaçabilir |
| 4 | 🪨 **Kök Golemi** | 0.3 (çok yavaş) | 2.5s | Tank, yavaş ama güçlü |
| 5 | ⚔️ **Labirent Muhafızı** | 0.7 | 1.8s | Boss, sadece son seviyelerde |

### Canavar Canları (HP)

**Formül:** `TypeBase + (seviye - 1) × TypeGrowth`

| Canavar | Base HP | Büyüme/Seviye | Sv.1 | Sv.2 | Sv.3 | Sv.4 | Sv.5 | Sv.6 | Sv.7 | Sv.8 | Sv.9 | Sv.10 |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| 🐛 Diken Böceği | 6 | +1 | 6 | 7 | 8 | 9 | 10 | 11 | 12 | 13 | 14 | 15 |
| 🦇 Gölge Yarasa | 9 | +1 | 9 | 10 | 11 | 12 | 13 | 14 | 15 | 16 | 17 | 18 |
| 🦊 Orman Tilkisi | 12 | +2 | 12 | 14 | 16 | 18 | 20 | 22 | 24 | 26 | 28 | 30 |
| 🪨 Kök Golemi | 18 | +2 | 18 | 20 | 22 | 24 | 26 | 28 | 30 | 32 | 34 | 36 |
| ⚔️ Labirent Muhafızı | 24 | +3 | 24 | 27 | 30 | 33 | 36 | 39 | 42 | 45 | 48 | 51 |

### Gereken Mandalina Vuruşu (HP ÷ 3)

| Canavar | Sv.1 | Sv.3 | Sv.5 | Sv.7 | Sv.10 |
|---|---|---|---|---|---|
| 🐛 Diken Böceği | **2** | 3 | 4 | 4 | **5** |
| 🦇 Gölge Yarasa | **3** | 4 | 5 | 5 | **6** |
| 🦊 Orman Tilkisi | **4** | 6 | 7 | 8 | **10** |
| 🪨 Kök Golemi | **6** | 8 | 9 | 10 | **12** |
| ⚔️ Labirent Muhafızı | **8** | 10 | 12 | 14 | **17** |

### Canavar Saldırı Hasarı (Oyuncuya)

**Formül:** `TypeBaseDMG + (seviye - 1) × TypeDMGGrowth`

| Canavar | Base DMG | Büyüme | Sv.1 | Sv.3 | Sv.5 | Sv.7 | Sv.10 |
|---|---|---|---|---|---|---|---|
| 🐛 Diken Böceği | 4 | +1 | **4** | 6 | 8 | 10 | **13** |
| 🦇 Gölge Yarasa | 6 | +1 | **6** | 8 | 10 | 12 | **15** |
| 🦊 Orman Tilkisi | 7 | +1.5 | **7** | 10 | 13 | 16 | **21** |
| 🪨 Kök Golemi | 10 | +2 | **10** | 14 | 18 | 22 | **28** |
| ⚔️ Labirent Muhafızı | 12 | +2 | **12** | 16 | 20 | 24 | **30** |

### Oyuncu Dayanıklılık Analizi (100 HP ile kaç vuruş yer?)

| Canavar | Sv.1 (vuruş) | Sv.5 (vuruş) | Sv.10 (vuruş) | Tehlike |
|---|---|---|---|---|
| 🐛 Diken Böceği | 25 vuruş | 12 vuruş | 7 vuruş | 🟢 Düşük |
| 🦇 Gölge Yarasa | 16 vuruş | 10 vuruş | 6 vuruş | 🟡 Orta (hızlı!) |
| 🦊 Orman Tilkisi | 14 vuruş | 7 vuruş | 4 vuruş | 🟡 Orta |
| 🪨 Kök Golemi | 10 vuruş | 5 vuruş | 3 vuruş | 🔴 Yüksek |
| ⚔️ Labirent Muhafızı | 8 vuruş | 5 vuruş | 3 vuruş | 🔴 Çok Yüksek |

---

## 🗺️ Seviye Bazlı Canavar Dağılımı

| Seviye | 🐛 Diken | 🦇 Yarasa | 🦊 Tilki | 🪨 Golem | ⚔️ Muhafız | Toplam |
|---|---|---|---|---|---|---|
| 1 | 2 | — | — | — | — | **2** |
| 2 | 2 | 1 | — | — | — | **3** |
| 3 | 1 | 1 | 1 | — | — | **3** |
| 4 | 2 | 1 | 1 | — | — | **4** |
| 5 | 1 | 1 | 1 | 1 | — | **4** |
| 6 | 2 | 1 | 1 | 1 | — | **5** |
| 7 | 1 | 2 | 1 | 1 | — | **5** |
| 8 | 1 | 2 | 2 | 1 | — | **6** |
| 9 | 1 | 2 | 1 | 1 | 1 | **6** |
| 10 | 1 | 2 | 2 | 1 | 1 | **7** |

> **Not:** İlk seviyelerde sadece kolay canavarlar çıkar. Yeni canavar türleri kademeli olarak tanıtılır.  
> Labirent Muhafızı (Boss) sadece **Seviye 9-10**'da çıkar.

---

## 🌳 Ağaç & Meyve Sistemi

### Mandalina Ağaçları

**Ağaç verimi:** `5 + seviye` mandalina/ağaç

| Seviye | Ağaç | Verim/Ağaç | Toplam Mandalina | Gereken (tüm canavarlar) | Fazla |
|---|---|---|---|---|---|
| 1 | 1 | 6 | **6** | 4 | +2 |
| 2 | 2 | 7 | **14** | 8 | +6 |
| 3 | 2 | 8 | **16** | 11 | +5 |
| 4 | 2 | 9 | **18** | 16 | +2 |
| 5 | 3 | 10 | **30** | 23 | +7 |
| 6 | 3 | 11 | **33** | 29 | +4 |
| 7 | 3 | 12 | **36** | 32 | +4 |
| 8 | 4 | 13 | **52** | 43 | +9 |
| 9 | 4 | 14 | **56** | 52 | +4 |
| 10 | 5 | 15 | **75** | 66 | +9 |

> **Denge notu:** Fazla mandalinalar puan bonusu olarak çalışır. Oyuncu tüm ağaçları bulmalı ama tüm canavarları öldürmek zorunda değil (kaçınabilir).

### Portakal Ağaçları

**Ağaç verimi:** 2-3 portakal/ağaç  
**İhtiyaç:** Çıkış kapısı açmak için **1 portakal** gerekli.

| Seviye | Ağaç | Verim/Ağaç | Toplam Portakal | Gereken | Fazla |
|---|---|---|---|---|---|
| 1-3 | 1 | 2 | **2** | 1 | +1 |
| 4-6 | 1 | 3 | **3** | 1 | +2 |
| 7-8 | 2 | 3 | **6** | 1 | +5 |
| 9-10 | 2 | 3 | **6** | 1 | +5 |

> Fazla portakallar = puan bonusu (kullanılmayan her portakal puan kazandırır).

### Limon Ağaçları

**Ağaç verimi:** Sv.1-6 → 2 limon/ağaç, Sv.7-10 → 3 limon/ağaç  
**Her limon = 1 soru hakkı**

| Seviye | Ağaç | Verim/Ağaç | Toplam Limon (Soru Hakkı) |
|---|---|---|---|
| 1 | 3 | 2 | **6** |
| 2 | 3 | 2 | **6** |
| 3 | 4 | 2 | **8** |
| 4 | 4 | 2 | **8** |
| 5 | 5 | 2 | **10** |
| 6 | 5 | 2 | **10** |
| 7 | 5 | 3 | **15** |
| 8 | 6 | 3 | **18** |
| 9 | 6 | 3 | **18** |
| 10 | 7 | 3 | **21** |

> Daha fazla limon kullanmak = daha fazla puan. Oyuncu limon aramaya teşvik edilir.

### Toplam Ağaç Sayıları (Harita Başına)

| Seviye | 🍊 Mandalina | 🍊 Portakal | 🍋 Limon | **Toplam Ağaç** |
|---|---|---|---|---|
| 1 | 1 | 1 | 3 | **5** |
| 2 | 2 | 1 | 3 | **6** |
| 3 | 2 | 1 | 4 | **7** |
| 4 | 2 | 1 | 4 | **7** |
| 5 | 3 | 1 | 5 | **9** |
| 6 | 3 | 1 | 5 | **9** |
| 7 | 3 | 2 | 5 | **10** |
| 8 | 4 | 2 | 6 | **12** |
| 9 | 4 | 2 | 6 | **12** |
| 10 | 5 | 2 | 7 | **14** |

---

## 🏆 Puanlama Sistemi

### Puan Kaynakları

| Puan Kaynağı | Hesaplama | Açıklama |
|---|---|---|
| ✅ **Seviye Tamamlama** | `500 + (seviye × 100)` | Seviyeyi bitirmek için temel puan |
| ⏱️ **Süre Bonusu** | `max(0, (ParSüresi - GeçenSüre)) × 3` | Her saniye erken bitirme = 3 puan |
| 🍊 **Mandalina Verimliliği** | `kullanılmayan × 20` | Az mandalina harcama ödülü |
| 🍊 **Portakal Verimliliği** | `kullanılmayan × 30` | Az portakal harcama ödülü |
| 🍋 **Limon Kullanımı** | `doğru_cevap × 50` | Soru çözme ödülü |
| 💀 **Ölüm Cezası** | `-(200 + seviye × 30)` | Ölünce puan kaybı |

### Par Süreleri (Hedef Tamamlama Süresi)

| Seviye | Par Süresi | Dakika |
|---|---|---|
| 1 | 120 saniye | 2 dk |
| 2 | 150 saniye | 2.5 dk |
| 3 | 180 saniye | 3 dk |
| 4 | 210 saniye | 3.5 dk |
| 5 | 240 saniye | 4 dk |
| 6 | 270 saniye | 4.5 dk |
| 7 | 300 saniye | 5 dk |
| 8 | 330 saniye | 5.5 dk |
| 9 | 360 saniye | 6 dk |
| 10 | 420 saniye | 7 dk |

### Ölüm Cezası Tablosu

| Seviye | Ölüm Cezası |
|---|---|
| 1 | -230 puan |
| 2 | -260 puan |
| 3 | -290 puan |
| 4 | -320 puan |
| 5 | -350 puan |
| 6 | -380 puan |
| 7 | -410 puan |
| 8 | -440 puan |
| 9 | -470 puan |
| 10 | -500 puan |

### Seviye Bazlı Maksimum Puan Analizi

> En iyi senaryo: Ölmeden, hızlı bitirip, tüm limonları doğru cevaplayıp, minimum meyve harcamak.

| Seviye | Tamamlama | Süre (60s erken) | Mandalina Fazla | Portakal Fazla | Limon (tümü doğru) | **Maks Puan** |
|---|---|---|---|---|---|---|
| 1 | 600 | 180 | 40 | 30 | 300 | **~1.150** |
| 2 | 700 | 180 | 120 | 30 | 300 | **~1.330** |
| 3 | 800 | 180 | 100 | 30 | 400 | **~1.510** |
| 4 | 900 | 180 | 40 | 60 | 400 | **~1.580** |
| 5 | 1.000 | 180 | 140 | 60 | 500 | **~1.880** |
| 6 | 1.100 | 180 | 80 | 60 | 500 | **~1.920** |
| 7 | 1.200 | 180 | 80 | 150 | 750 | **~2.360** |
| 8 | 1.300 | 180 | 180 | 150 | 900 | **~2.710** |
| 9 | 1.400 | 180 | 80 | 150 | 900 | **~2.710** |
| 10 | 1.500 | 180 | 180 | 150 | 1.050 | **~3.060** |

### 🏅 Toplam Oyun Maksimum Puanı: **~20.210**

| Derece | Puan Aralığı | Seviye |
|---|---|---|
| 🥇 S Rank | 18.000+ | Efsanevi |
| 🥈 A Rank | 14.000 - 17.999 | Harika |
| 🥉 B Rank | 10.000 - 13.999 | İyi |
| 📋 C Rank | 6.000 - 9.999 | Orta |
| 📋 D Rank | 0 - 5.999 | Geliştirilmeli |

---

## 📊 Seviye Özet Tablosu (Hepsi Bir Arada)

| Sv. | Canavar | Ağaç | Mandalina | Portakal | Limon | Par Süre | Tamamlama Puanı | Ölüm Cezası |
|---|---|---|---|---|---|---|---|---|
| 1 | 2 | 5 | 6 | 2 | 6 | 2 dk | 600 | -230 |
| 2 | 3 | 6 | 14 | 2 | 6 | 2.5 dk | 700 | -260 |
| 3 | 3 | 7 | 16 | 2 | 8 | 3 dk | 800 | -290 |
| 4 | 4 | 7 | 18 | 3 | 8 | 3.5 dk | 900 | -320 |
| 5 | 4 | 9 | 30 | 3 | 10 | 4 dk | 1.000 | -350 |
| 6 | 5 | 9 | 33 | 3 | 10 | 4.5 dk | 1.100 | -380 |
| 7 | 5 | 10 | 36 | 6 | 15 | 5 dk | 1.200 | -410 |
| 8 | 6 | 12 | 52 | 6 | 18 | 5.5 dk | 1.300 | -440 |
| 9 | 6 | 12 | 56 | 6 | 18 | 6 dk | 1.400 | -470 |
| 10 | 7 | 14 | 75 | 6 | 21 | 7 dk | 1.500 | -500 |

---

## ⚖️ Denge Felsefesi

### Zorluk Eğrisi
```
Zorluk
  ▲
  │                              ╱ Sv.10
  │                          ╱
  │                      ╱
  │                 ╱        ← Kademeli artış
  │            ╱
  │       ╱
  │  ╱  Sv.1
  └──────────────────────────► Seviye
```

### Strateji Dengesi
- 🍋 **Limon topla** → Daha çok soru çöz → Daha çok puan + yön ipucu
- 🍊 **Mandalina biriktir** → Sadece gereken canavarları öldür → Verimlilik puanı
- ⏱️ **Hızlı ol** → Süre bonusu → Ama keşif yapamayabilirsin
- 🧭 **Keşfet** → Daha çok ağaç bul → Ama süre kaybedersin

> **Sonuç:** Oyuncu her seviyede "hız vs keşif" arasında strateji kararı vermeli.

---

> ⚠️ **Bu doküman taslaktır.** Tüm değerler test edilerek ayarlanacaktır. Playtest sonrası dengeleme yapılacak.
