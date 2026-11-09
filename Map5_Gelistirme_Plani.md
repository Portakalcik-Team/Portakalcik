# Portakalcık — Harita 5 “Spiral” Geliştirme Planı

Bu belge Harita 5'in ders haftalarına yayılan küçük ve gösterilebilir adımlarla geliştirilmesi için hazırlanmıştır.

## 1. hafta — İlk duvarlar

- Küçük bir zemin parçası
- Spiral fikrini gösterecek birkaç kısa duvar
- Basit başlangıç ve çıkış işaretleri

Duvarlar Unity sahnesine normal GameObject olarak elle eklenir. Bu haftada otomatik
harita üretme kodu, oynanış kodu, canavar, ağaç, kapı, ses veya final görselleri yoktur.

## 2. hafta — Spiral labirent + ilk scriptler (W02)

### Labirent
- 1. haftanın 4 duvarı yerinde kaldı ve spiralin merkezi oldu. Spiral bu duvarlardan dışa doğru büyüdü.
- Zemin 36×36 m'ye büyütüldü. Halkalar arası 4,5 m; koridorlar 3,5 m geniş, duvarlar 3 m yüksek.
- Dış halkaların köşeleri 45° kırık, böylece spiral yuvarlağa yakın görünür.
- Giriş sağ üst köşede. Alt halkadaki boşluk doğru yol; spirali dolaşmaya devam etmek çıkmaza gider.

### Scriptler (`Assets/Project/Scripts/Map5/`)
| Script | Nerede | Ne yapar |
|---|---|---|
| `Map5Spinner` | 3 meyvenin `Visual` çocuğu | Kendi etrafında döner (saniyede 90°, `Time.deltaTime`) |
| `Map5Bobber` | 3 meyvenin `Visual` çocuğu | Yukarı aşağı süzülür (`Time.time` ile), hızları 2 / 1,6 / 2,4 |
| `Map5Patroller` | `Tehlike_Devriye_01` | A ile B noktası arasında gidip gelir, alt boşluğun önünden geçer |
| `Map5PlayerMover` | `Oyuncu` | Geçici oyuncu kutusu W A S D ile yürür, duvarlardan geçemez |

- Hareket eden her nesne boş bir kök + `Visual` çocuk şeklinde kuruldu (3. haftada köke fizik gelecek).
- Tehlikenin B noktası tehlikenin çocuğu değil, `Tehlikeler` altında ayrı bir boş nesne.

### Fiiller (Verbs)
| Fiil | Karşılaştığı şeyler |
|---|---|
| yürü | duvarlar, çıkmaz yollar, devriye tehlikesi |
| topla | mandalina, portakal, limon |
| at (mandalina) | Diken Böceği, Yarasa, Tilki |
| aç | kısayol kapısı, çıkış kapısı, Kök Golemi'nin kısayolu |

## Sonraki haftalar

| Hafta | Eklenecek ayrıntı |
|---|---|
| 2 | Koridor oranları, duvar yüksekliği ve dört kısayol alanı |
| 3 | Oyuncu hareketi, kamera ve çarpışmalar |
| 4 | Kapılar ve kısayol açılma sistemi |
| 5 | Mandalina, portakal ve limon ağaçlarının geçici modelleri |
| 6 | Diken Böceği, Yarasa ve Tilki yerleşimleri |
| 7 | Kök Golemi ve merkezdeki büyük kısayol |
| 8 | Tavan, atmosferik ışık, ses ve çevre detayları |
| 9 | Denge, dört dakikalık hedef süre ve performans testi |
| 10 | Son görsel düzenleme ve hata düzeltmeleri |

İlk hafta özellikle küçük tutulur. Haritanın tam ölçüsü ve bütün spiral rota
ikinci haftadan itibaren duvarlar tek tek çoğaltılarak oluşturulur.

---

## 3. hafta — Fizik, tetikler, taşıma (W03)

- Oyuncu: `Player` tag'i + Rigidbody (Freeze Rotation X/Z).
- `Map5PlayerTrigger`: Algıla → Kim? → Yap. Meyveler ("toplandı!", kaybolur), kırmızı tehlike ("Ah!"), yeşil çıkış ("Bölüm tamamlandı!").
- Taşıma: `Map5Cargo`, `Map5PlayerCarry` (+ `TasimaNoktasi`), `Map5DeliveryZone` (yeşil teslim), `Map5Hazard` (kargoyu düşürür).
- Prefab'lar: `Map5_Meyve`, `Map5_Tehlike`, `Map5_Kargo`. Buz: `M_Map5_Buz` (görünüş) + `PM_Map5_Buz` (sürtünme 0.02, Minimum).

## 4. hafta — Input System ile hareket (W04)

- `Map5PlayerMove` (WASD / çubuk, fizikle, gittiği yöne döner), `Map5PlayerJump` (Space, sadece Ground'dan), `Map5Slippery` (buzda tutuş düşer), `Map5PlayerDrop` (E).
- Interpolate + sürtünmesiz `PM_Map5_Oyuncu`, Ground katmanı (Layer 3).

### Feel notları
| Ayar | Değer | Neden |
|---|---|---|
| Max Speed | 5 | Labirent dar, daha hızlısı duvara çarptırıyor |
| Acceleration | 40 | Tam hıza 0.14 sn'de çıkıyor |
| Turn Speed | 720 | Büyücü yeni yöne hızlı dönüyor |
| Jump Force | 4 | Yaklaşık 0.8 m, kırmızı şeridi geçmeye yetiyor |
| Buz Grip | 0.04 | Rampaya sadece hızlı ve düz çıkılıyor |

## 5. hafta — Cinemachine kamera (W05)

- `Map5_Week5_CM_Follow` büyücüyü takip eder (Position Composer). Main Camera'da Cinemachine Brain.
- Sarsıntı: kamerada Impulse Listener; `Map5_Tehlike` prefab'ı, devriye tehlikesi ve hayalette Impulse Source. `Map5Hazard` ve `Map5Canavar` kargo düşünce `GenerateImpulse()` çağırır.
- `Map5_Week5_CM_Ramp` + `Map5_Week5_RampZone` (`Map5CameraZone`): rampaya girince yukarıdan bakan kamera.

### Kamera notları
| Ayar | Slayt | Map5 | Neden |
|---|---|---|---|
| Açı (Rotation X) | 40 | 55 | Duvarlar 3 m, alçak açıda büyücüyü saklıyor |
| Camera Distance | 14 | 16 | Labirentte bir sonraki dönemeç görünsün |
| Damping | 1 | 1 | Sakin ama geride kalmıyor |
| Dead Zone | 0.1 × 0.1 | 0.1 × 0.1 | Zıplayınca kamera oynamıyor |
| Lookahead | 0.4 s | 0.4 s | 5 m/s × 0.4 = 2 m önü görülüyor |
| Sarsıntı | 0, -0.5, 0 · 0.2 s | aynı | Sadece kargo düşünce / hayalet yakalayınca |
| Rampa kamerası | 60° · 16 m | 70° · 20 m | Köşedeki tırmanışın tamamı görünsün |
| Geçiş (Blend) | Ease In Out 1 s | aynı | 2 s yavaştı |

## 6. hafta — Kargo pedleri, hafif/ağır kargo, skor (W06)

- `Map5CargoSpawner`: pedin üstünde kargo yoksa geri sayar, süre bitince `Instantiate` ile yeni kargo yapar (kaynak).
- `Map5CargoType` (ScriptableObject): `Data/Map5/CT_Map5_Hafif` ve `CT_Map5_Agir` dosyaları.
- `Map5_Kargo_Agir`: `Map5_Kargo`'nun prefab variant'ı (turuncu `M_Map5_Agir`, daha büyük, tür = ağır).
- `Map5DeliveryZone` skoru tutar (teslim alanı = gider/sink). `Map5PlayerCarry` ağır kargoda oyuncunun kütlesini artırır.
- Eski 3 kargo yerine 3 hafif ped (`Map5_Week6_Ped_01-03`), spiralin içine 1 ağır ped (`Map5_Week6_Ped_Agir`).

### Denge notları (Balance notes)
| Ayar | Hafif | Ağır | Neden |
|---|---|---|---|
| Puan | 1 | 3 | Ağır riskli, ödülü büyük olmalı |
| Ağırlık (kg) | 0 | 0.3 | Slayttaki 1 kg ile ağır kargo spiralden hiç çıkamıyordu (kırmızı şeritler koridoru kapatıyor). 0.3 ile zıplama 0.8 m'den ~0.5 m'ye düşüyor: şeridi geçmek mümkün ama zamanlama gerekiyor |
| Yeniden gelme | 4 s | 8 s | Ağırı beklemek de bir maliyet |
| Yer | açık alan (teslimin yanı, güney şerit, platform) | spiralin içi, devriye tehlikesinin yanı | Ağırı almak için tehlike + hayalet + şerit zıplaması |

Oyun testi: (testi yapınca buraya yaz: kaç hafif, kaç ağır teslim etti, neden)
