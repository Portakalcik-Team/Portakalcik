# Map3 W05 ve W06

Bu çalışma W05 kamera ve W06 kargo derslerinin Map_3 uygulamasıdır. Yeni sahne nesneleri Map3_W05_W06 altında bulunur. Konumlar mevcut Map_3 yerleşimine uyarlandı.

## W05 — Feel notes

| Ayar | Değer |
|---|---|
| Map3_CM_Follow dönüşü | 40, 0, 0 |
| Görüş açısı ve takip mesafesi | 60 derece, 14 birim |
| Damping | 1, 1, 1 |
| Dead Zone ve derinliği | 0.1 × 0.1, 1 |
| Lookahead | 0.4 saniye, smoothing 10, Ignore Y açık |
| Sarsıntı | Uniform, Bump, 0.2 saniye, hız 0, -0.5, 0 |
| Impulse Listener | Gain 1, Camera Space açık, Apply After Noise |
| Map3_CM_Ramp | 60, 0, 0 dönüş, 16 birim mesafe |
| Kamera geçişi | Ease In Out, 1 saniye |
| Map3_RampZone | Avludaki rampa ve platform, merkez 9, 2, -21, boyut 14, 6, 8 |

Takip kamerası oyuncuya yaklaşır. Damping yumuşak takip sağlar. Dead Zone küçük hareketlere tepkiyi azaltır. Lookahead hareket yönünde görüş alanı bırakır ve düşey hareketi hesaba katmaz. Rampadaki kamera daha dik açıyla tırmanışı gösterir. Sarsıntı yalnızca tehlike taşınan kargoyu düşürdüğünde oluşur.

Main Camera üzerindeki önceki M3KameraTakip bileşeni kapalıdır. Kamerayı Cinemachine Brain yönetir. Map3_CM_Ramp başlangıçta kapalıdır. Kurulu Cinemachine 6.6.0 sürümünün slayttaki bileşenleri kullanıldı.

## W06 — Balance notes

| Tür | Puan | Ek kütle | Yenilenme | Üretim noktası |
|---|---|---|---|---|
| Map3_CT_Light | 1 | 0 | 4 saniye | Üç mevcut kargo konumu |
| Map3_CT_Heavy | 3 | 1 | 8 saniye | İki mevcut tehlikenin arası |

Üreticiler oyun başında birer kargo oluşturur. Kargo üreticinin altında beklerken sayaç ilerlemez. Oyuncu kargoyu alınca geri sayım başlar.

Map3_Cargo_Heavy, Map3_Kargo prefabının varyantıdır. Görseli E07B1A turuncu rengindedir. Görsel ölçeği 1, 1, 1, salınım yüksekliği 0.15 ve salınım hızı 1'dir. Beyaz üretim disklerinde Collider bulunmaz.

Oyuncunun temel kütlesi 1'dir. Ağır kargoyu taşıyınca 2 olur. Aynı zıplama kuvveti daha küçük yükseliş sağlar. Yürüme hızını hareket kodu belirler. E ile bırakma, tehlikenin düşürmesi ve teslim etme kütleyi eski değerine getirir. Teslim alanı puanı Console'a yazar. Mevcut iki teslim alanı kendi puanını ayrı tutar.

## Unity içinde doğrulananlar

- Oyun başında dört kargo üretildi.
- Fizik tetikleyicileriyle hafif ve ağır kargo alındı.
- Hafif üretici 4, ağır üretici 8 saniye sonra yeniden doldu.
- Aynı teslim alanına hafif ve ağır teslim edilince Console toplamı 1 ve ardından 4 oldu.
- Ağır kargoda kütle 2 oldu. Teslimde ve bırakmada 1'e döndü.
- Space girdisi normal ve ağır kargolu zıplamayı çalıştırdı. Aynı örnekleme anında düşey hızlar yaklaşık 3.41 ve 1.41 olarak ölçüldü.
- E girdisi kargoyu bıraktı ve kamera sarsıntısı üretmedi.
- Sabit tehlike ve hareketli canavar kargoyu düşürüp kısa kamera sarsıntısı üretti.
- Rampa bölgesine girişte ikinci kamera, çıkışta takip kamerası devreye girdi.
- Sahne denetiminde eksik script bulunmadı.

## Sınavda gösterim sırası

1. Map_3 sahnesini aç, Play'e bas ve Game görünümüne tıkla.
2. WASD ile hareket et, Space ile zıpla. Takibi ve kamera ayarlarını göster.
3. Kargoya yaklaşarak al. Üretim noktasına 4 saniye sonra yenisinin geldiğini göster.
4. Kargoyu teslim alanına götür ve Console'daki puanı göster.
5. Ağır turuncu kargoyu al. Küçülen zıplamayı ve 3 puanlık teslimi göster.
6. E ile bırakma ile tehlikeye çarpmanın kamera üzerindeki farkını göster.
7. Avludaki rampaya girip çık. İkinci kameraya yumuşak geçişi göster.
8. Map3_CT_Light ve Map3_CT_Heavy veri dosyalarını ve ağır prefabın varyant bağlantısını göster.

## Gerçek oyuncu denemesi

Slaytların istediği yeni bir oyuncuyla deneme henüz yapılmadı. Otomatik kontrollerin sonuçları yukarıdadır. Bu bölüm gerçek denemeden sonra doldurulmalıdır.

- W05 — Oyuncunun tehlikeyi geç fark ettiği konumlar ve kamera hakkındaki gözlemleri.
- W06 — İki dakikadaki hafif teslim sayısı ve ağır teslim sayısı.
- Oyuncunun kargo tercihinin nedeni.
- Değer değiştirildiyse değişen değer ve ikinci denemenin sonucu.
