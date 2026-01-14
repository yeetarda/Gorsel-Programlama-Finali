using System;
using System.Collections.Generic;
using System.Linq; 
using System.Net.Http; 
using System.Text.Json; 
using System.Threading.Tasks; 

namespace CurrencyTracker
{

    public class CurrencyResponse
    {
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; }
        public decimal Rate { get; set; }
    }

    class Program
    {
        private static readonly string ApiUrl = "https://api.frankfurter.app/latest?from=TRY";

        static async Task Main(string[] args)
        {
            Console.Title = "Döviz Takip Konsol Uygulaması";

            Console.WriteLine("Veriler çekiliyor, lütfen bekleyiniz...");
            List<Currency> currencyList = await GetCurrenciesAsync();

            if (currencyList == null || currencyList.Count == 0)
            {
                Console.WriteLine("Veri çekilemedi veya liste boş. Program kapatılıyor.");
                return;
            }

            Console.Clear();

            while (true)
            {
                Console.WriteLine("\n===== CurrencyTracker =====");
                Console.WriteLine("1. Dövizleri listele");
                Console.WriteLine("2. İsme döviz ara");
                Console.WriteLine("3. Girilen değerden büyük dövizleri listele");
                Console.WriteLine("4. Dövizleri sırala");
                Console.WriteLine("5. İstatistiksel özet göster");
                Console.WriteLine("0. Çıkış");
                Console.Write("Seçiminiz: ");

                string secim = Console.ReadLine();

                switch (secim)
                {
                    case "1":
                        Listele(currencyList);
                        break;
                    case "2":
                        Ara(currencyList);
                        break;
                    case "3":
                        DegerdenBuyukleriListele(currencyList);
                        break;
                    case "4":
                        Sirala(currencyList);
                        break;
                    case "5":
                        IstatistikGoster(currencyList);
                        break;
                    case "0":
                        Console.WriteLine("Çıkış yapılıyor...");
                        return; 
                    default:
                        Console.WriteLine("Geçersiz seçim, tekrar deneyin.");
                        break;
                }
            }
        }

        private static async Task<List<Currency>> GetCurrenciesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    HttpResponseMessage response = await client.GetAsync(ApiUrl);

                    response.EnsureSuccessStatusCode();

                    string jsonString = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

              
                    CurrencyResponse apiResponse = JsonSerializer.Deserialize<CurrencyResponse>(jsonString, options);

                   
                    List<Currency> liste = apiResponse.Rates
                                          .Select(x => new Currency { Code = x.Key, Rate = x.Value })
                                          .ToList();

                    return liste;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                return new List<Currency>();
            }
        }

     
        private static void Listele(List<Currency> currencies)
        {
            Console.WriteLine("\n--- Tüm Dövizler ---");
            // Sadece listeyi ekrana basıyoruz
            foreach (var item in currencies)
            {
                Console.WriteLine($"{item.Code}: {item.Rate}");
            }
        }

       
        private static void Ara(List<Currency> currencies)
        {
            Console.Write("Aranacak Döviz Kodu (Örn: USD, EUR): ");
            string input = Console.ReadLine().ToUpper(); 

            var sonuc = currencies.Where(c => c.Code == input).ToList();

            if (sonuc.Count > 0)
            {
                foreach (var item in sonuc)
                {
                    Console.WriteLine($"Bulundu -> {item.Code}: {item.Rate}");
                }
            }
            else
            {
                Console.WriteLine("Böyle bir döviz kodu bulunamadı.");
            }
        }

        private static void DegerdenBuyukleriListele(List<Currency> currencies)
        {
            Console.Write("Bir değer giriniz (Örn: 0.5): ");
            if (decimal.TryParse(Console.ReadLine(), out decimal deger))
            {
                
                var sonuclar = currencies.Where(c => c.Rate > deger).ToList();

                Console.WriteLine($"\n{deger} değerinden büyük olan {sonuclar.Count} kur bulundu:");
                foreach (var item in sonuclar)
                {
                    Console.WriteLine($"{item.Code}: {item.Rate}");
                }
            }
            else
            {
                Console.WriteLine("Lütfen geçerli bir sayı giriniz.");
            }
        }

        private static void Sirala(List<Currency> currencies)
        {
            Console.WriteLine("1- Artan Sıralama (Küçükten Büyüğe)");
            Console.WriteLine("2- Azalan Sıralama (Büyükten Küçüğe)");
            Console.Write("Seçim: ");
            string secim = Console.ReadLine();

            List<Currency> siraliListe;

            if (secim == "1")
            {

                siraliListe = currencies.OrderBy(c => c.Rate).ToList();
            }
            else
            {
                siraliListe = currencies.OrderByDescending(c => c.Rate).ToList();
            }

            Console.WriteLine("\n--- Sıralı Liste ---");
            foreach (var item in siraliListe)
            {
                Console.WriteLine($"{item.Code}: {item.Rate}");
            }
        }
        private static void IstatistikGoster(List<Currency> currencies)
        {
            if (currencies.Count == 0) return;

            int toplamSayi = currencies.Count();
            decimal enYuksek = currencies.Max(c => c.Rate);
            decimal enDusuk = currencies.Min(c => c.Rate);
            decimal ortalama = currencies.Average(c => c.Rate);

            var enYuksekKurObj = currencies.First(c => c.Rate == enYuksek);
            var enDusukKurObj = currencies.First(c => c.Rate == enDusuk);

            Console.WriteLine("\n--- İstatistikler ---");
            Console.WriteLine($"Toplam Döviz Sayısı : {toplamSayi}");
            Console.WriteLine($"En Yüksek Kur       : {enYuksek} ({enYuksekKurObj.Code})");
            Console.WriteLine($"En Düşük Kur        : {enDusuk} ({enDusukKurObj.Code})");
            Console.WriteLine($"Ortalama Değer      : {Math.Round(ortalama, 4)}");
        }
    }
}