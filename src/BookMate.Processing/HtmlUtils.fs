namespace BookMate.Processing

module HtmlUtils = 
  open System.IO
  open HtmlAgilityPack

  let loadHtml (fileText : string) = 
        let html = new HtmlAgilityPack.HtmlDocument()
        html.OptionWriteEmptyNodes <- true
        html.OptionOutputAsXml <- true
        html.LoadHtml(fileText)
        html

  let getTextFromHtml (html : HtmlDocument) = 
      let textNodes = html.DocumentNode.SelectNodes("//p//text()")
      
      let text = 
          seq { 
              for p in textNodes do
                  if p :? HtmlTextNode then 
                      let node = p :?> HtmlTextNode
                      node.Text <- node.Text
                      yield node.Text
          }
          |> Array.ofSeq
      text

  let saveHtml (html : HtmlDocument) (savePath : string) = 
      let mutable result = null
      use writer = new StringWriter()
      html.Save(writer)
      result <- writer.ToString()
      File.WriteAllText(savePath, result)

  let updateTextNode (node : HtmlTextNode) : HtmlTextNode = 
    let updatedNode = (node.CloneNode true) :?> HtmlTextNode
    updatedNode.Text <- updatedNode.Text + " {{Hi fuck you}}"
    updatedNode