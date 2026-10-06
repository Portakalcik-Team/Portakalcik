# 🗺️ MapGenerator — Harita Otomatik Oluşturma Rehberi

> **Portakalcık Projesi**  
> Bu rehber, tüm ekip arkadaşlarının kendi seviye haritalarını otomatik olarak Unity'de oluşturabilmesi için hazırlanmıştır.

---

## 🚀 Önemli Uyarı: Çakışmaları Önlemek

Ekip olarak çalışırken herkesin kendi `MapGenerator.cs` script'ini oluşturması veya kopyalaması **Unity'de derleme hatalarına (class name conflict) yol açar.** 

Bu yüzden script'i kopyalamayacağız! Projede herkesin kullanabileceği ortak bir script oluşturduk.

**Ortak Script Konumu:** `Assets/Project/Scripts/Shared/MapGenerator.cs`

Bunu kendi sahnende nasıl kullanacağını aşağıda adım adım anlattım.

---

## 🏗️ Adım 1: Prefab Oluşturma (Her Seviye İçin)

Kendi haritanda kullanacağın prefab'ları (duvar, kapı, düşman vb.) hazırlamalısın. İstersen ekip arkadaşlarının yaptığı prefab'ları (örneğin `Assets/Project/Prefabs/Map4` içindeki) doğrudan kullanabilirsin. Eğer kendi özel tasarımlarını (farklı renk vb.) yapmak istersen şu adımları izle:

1. **Hierarchy** → sağ tık → `3D Object > Cube`
2. **İsim** ver ve **Scale** değiştir (aşağıdaki tabloya bak)
3. **Material** sürükle (`Assets/Project/Art/Materials/` klasöründen)
4. Gerekiyorsa **BoxCollider > Is Trigger** ✅ işaretle
5. **Hierarchy'den → Kendi Prefab klasörüne (ör: `Assets/Project/Prefabs/Map5_Mehmet`) sürükle** = Prefab olur! 
6. Sahnedeki objeyi **sil** (prefab Project'te kalır)

### 📋 Kullanılması Gereken Prefab'lar

| # | İsim | Scale (X, Y, Z) | Material | Is Trigger |
|---|------|-----------------|----------|-----------|
| 1 | `Wall Prefab` | `3, 3, 3` | `Walls` | ❌ |
| 2 | `Path Prefab` | `3, 0.1, 3` | `Floor` | ❌ |
| 3 | `Player Prefab` | `1, 2, 1` | `Start` | ❌ |
| 4 | `Exit Prefab` | `2, 3, 2` | `Exit` | ❌ |
| 5 | `Door Prefab` | `3, 3, 0.5` | `Doors` | ✅ |
| 6 | `Limon Prefab` | `1, 1.5, 1` | `Agaclar` | ✅ |
| 7 | `Mandalina Prefab` | `1.2, 1.8, 1.2` | `Agaclar` | ✅ |
| 8 | `Portakal Prefab` | `1.5, 2, 1.5` | `Agaclar` | ✅ |
| 9 | `Diken Bocegi Prefab` | `1, 0.5, 1.5` | `Enemy` | ✅ |
| 10 | `Golge Yarasa Prefab` | `1, 0.8, 1` | `Enemy` | ✅ |
| 11 | `Orman Tilkisi Prefab` | `1, 1, 2` | `Enemy` | ✅ |
| 12 | `Kok Golemi Prefab` | `2, 2.5, 2` | `Enemy` | ✅ |
| 13 | `Labirent Muhafiz Prefab` | `2.5, 3, 2.5` | `Enemy` | ✅ |

---

## 🔧 Adım 2: LevelBuilder Objesi Oluşturma

Kendi sahnene (ör: `Assets/Scenes/Map_5.unity`) girdikten sonra:

1. **Hierarchy** → sağ tık → `Create Empty`
2. İsim: `LevelBuilder`
3. Position: `(0, 0, 0)` olduğundan emin ol.

### Ortak Script'i Ekle
1. `LevelBuilder` seçili iken
2. **Inspector** → `Add Component` butonuna tıkla
3. Arama kutusuna `MapGenerator` yaz ve seç.
*(Eğer çıkmazsa, Project panelinden `Assets/Project/Scripts/Shared/MapGenerator.cs` dosyasını sürükleyip Inspector'a bırakabilirsin).*

---

## 🔗 Adım 3: Prefab'ları Bağla

Inspector'da `MapGenerator` component'inin altında boş prefab slotları göreceksin. Hazırladığın (veya takımın ortak hazırladığı) prefab'ları Project panelinden bu slotlara sürükle-bırak yap:

- `wallPrefab` → Duvar prefab'ını sürükle
- `pathPrefab` → Yol prefab'ını sürükle
- *(Tüm düşman, ağaç, kapı vb. prefab'larını ilgili yerlere sürükle)*

---

## 🗺️ Adım 4: Haritayı Oluştur!

1. Kendi haritanın text tasarımını kopyala (GDD'den veya `Portakalcik_Harita_Tasarimlari.md` içinden).
2. Inspector'daki **`mapText`** alanına yapıştır.
3. Inspector'ın en altındaki **🗺️ Haritayı Oluştur** butonuna bas!

> **Not:** `█` karakterleri (dolu blok) haritada duvarları temsil eder.

---

## ❗ Sık Karşılaşılan Sorunlar

- **"mapText boş!" hatası:** Inspector'daki `mapText` alanına harita metnini yapıştırmayı unutmuşsun.
- **"Prefab atanmamış!" uyarısı:** İlgili prefab slotuna prefab'ı sürükle-bırak yapmamışsın.
- **Objeler çok büyük/küçük:** `cellSize` değerini değiştir. Varsayılan `3`'tür.
- **Objeler yanlış yerde:** `LevelBuilder` objesinin Transform Position değeri `(0, 0, 0)` olmalı.
- **"The type or namespace name 'MapGenerator' could not be found":** Yanlışlıkla kendi klasörüne yeni bir script oluşturmuş olabilirsin. Kendi scriptini silip `Assets/Project/Scripts/Shared/MapGenerator.cs` kullan.

---

## 👥 Kim Hangi Seviyeyi Yapıyor?

| Seviye | Sorumlu | Sahne (Scene) | Durum |
|--------|---------|---------------|-------|
| 1 | — | `Map_1` | — |
| 2 | — | `Map_2` | — |
| 3 | — | `Map_3` | — |
| 4 | AhmetCan | `Map_4` | 🔄 Devam ediyor |
| 5 | — | `Map_5` | — |
| 6 | — | `Map_6` | — |
| 7 | — | `Map_7` | — |
| 8 | — | `Map_8` | — |
| 9 | — | `Map_9` | — |
| 10 | — | `Map_10` | — |

*Bu rehber Portakalcık projesi için çakışmaları önleyecek şekilde güncellenmiştir.*
