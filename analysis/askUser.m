function answer = askUser(word, currentIter, wordCount)
  userAsnwered = false;
  while ~userAsnwered 
    prompt = sprintf('%d/%d: "%s" (1/0)? ', currentIter, wordCount, word);
    x = input(prompt);
    if (x == 1) 
      userAsnwered = true;
    elseif (x == 0)
      userAsnwered = true;
    end
  end
  answer = x; 
endfunction  