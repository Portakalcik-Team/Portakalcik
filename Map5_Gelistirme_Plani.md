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
