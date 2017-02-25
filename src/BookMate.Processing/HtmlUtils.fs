namespace BookMate.Processing

module HtmlUtils = 
  open System.IO
  open HtmlAgilityPack
  
  let getTextNodes (html : HtmlDocument) = html.DocumentNode.SelectNodes("//p//text()")
  
  let loadHtml (fileText : string) = 
      let html = new HtmlAgilityPack.HtmlDocument()
      html.OptionWriteEmptyNodes <- true
      html.OptionOutputAsXml <- true
      html.LoadHtml(fileText)
      html
  
  let getTextFromHtml (html : HtmlDocument) = 
      let textNodes = getTextNodes html
      
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
  
  let updateTextNode (node : HtmlTextNode) (newText : string) : HtmlTextNode = 
      let updatedNode = (node.CloneNode true) :?> HtmlTextNode
      updatedNode.Text <- newText
      updatedNode
  
  let cloneHtmlDocument (html : HtmlDocument) : HtmlDocument = 
      let mutable result = null
      use writer = new StringWriter()
      html.Save(writer)
      result <- writer.ToString()
      loadHtml result
  
  let processNodes (html : HtmlDocument) : HtmlDocument = 
      let updatedHtml = cloneHtmlDocument html
      let textNodes = getTextNodes updatedHtml
      for p in textNodes do
          if p :? HtmlTextNode then 
              let node = p :?> HtmlTextNode
              let updatedNode = updateTextNode node "{{Test}}"
              p.ParentNode.ReplaceChild(updatedNode, p) |> ignore
      updatedHtml
