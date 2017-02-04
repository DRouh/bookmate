namespace BookMate.Integration.Tests

module YandexMocks = 
  open BookMate.Integration.Yandex
  let expectedDictionaryResponse = 
        { head = seq []
          def = 
              [| { text = "time"
                   pos = "noun"
                   ts = "taɪm"
                   tr = 
                       [| { text = "время"
                            pos = "noun"
                            syn = 
                                [| { text = "раз" }
                                   { text = "момент" }
                                   { text = "срок" }
                                   { text = "пора" }
                                   { text = "период" } |]
                            mean = 
                                [| { text = "period" }
                                   { text = "once" }
                                   { text = "moment" }
                                   { text = "pore" } |]
                            ex = 
                                [| { text = "daylight saving time"
                                     tr = [| { text = "летнее время" } |] }
                                   { text = "take some time"
                                     tr = [| { text = "занять некоторое время" } |] }
                                   { text = "real time mode"
                                     tr = [| { text = "режим реального времени" } |] }
                                   { text = "expected arrival time"
                                     tr = [| { text = "ожидаемое время прибытия" } |] }
                                   { text = "external time source"
                                     tr = [| { text = "внешний источник времени" } |] }
                                   { text = "next time"
                                     tr = [| { text = "следующий раз" } |] }
                                   { text = "initial time"
                                     tr = [| { text = "начальный момент" } |] }
                                   { text = "reasonable time frame"
                                     tr = [| { text = "разумный срок" } |] }
                                   { text = "winter time"
                                     tr = [| { text = "зимняя пора" } |] }
                                   { text = "incubation time"
                                     tr = [| { text = "инкубационный период" } |] } |] }
                          { text = "час"
                            pos = "noun"
                            syn = null
                            mean = [| { text = "hour" } |]
                            ex = 
                                [| { text = "checkout time"
                                     tr = [| { text = "расчетный час" } |] } |] }
                          { text = "эпоха"
                            pos = "noun"
                            syn = null
                            mean = [| { text = "era" } |]
                            ex = null }
                          { text = "век"
                            pos = "noun"
                            syn = null
                            mean = [| { text = "age" } |]
                            ex = null }
                          { text = "такт"
                            pos = "noun"
                            syn = [| { text = "темп" } |]
                            mean = 
                                [| { text = "cycle" }
                                   { text = "rate" } |]
                            ex = null }
                          { text = "жизнь"
                            pos = "noun"
                            syn = null
                            mean = [| { text = "life" } |]
                            ex = null } |] }
                 { text = "time"
                   pos = "verb"
                   ts = "taɪm"
                   tr = 
                       [| { text = "приурочивать"
                            pos = "verb"
                            syn = null
                            mean = null
                            ex = null }
                          { text = "рассчитывать"
                            pos = "verb"
                            syn = null
                            mean = [| { text = "count" } |]
                            ex = null } |] }
                 { text = "time"
                   pos = "adjective"
                   ts = "taɪm"
                   tr = 
                       [| { text = "временный"
                            pos = "adjective"
                            syn = [| { text = "временной" } |]
                            mean = [| { text = "temporary" } |]
                            ex = 
                                [| { text = "time series model"
                                     tr = [| { text = "модель временных рядов" } |] }
                                   { text = "time correlation function"
                                     tr = [| { text = "временная корреляционная функция" } |] }
                                   { text = "time code"
                                     tr = [| { text = "временной код" } |] } |] }
                          { text = "повременный"
                            pos = "adjective"
                            syn = null
                            mean = null
                            ex = 
                                [| { text = "time payment"
                                     tr = [| { text = "повременная оплата" } |] } |] } |] } |] }