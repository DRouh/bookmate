function value = getNextWordToTest(p)
  %min - for the words with the greatest count
  %max - for the words with the smallest count
    
  %find poin near which we seek next word to ask
  %!!!if the word was true - go right, if not then go left
  notTestedWords = p(find(p ~= 1 & p ~= 0));
  meanP = mean(notTestedWords);
  unansweredWordProbabilities = p(find(p ~= 1 & p ~= 0 & meanP > p)); 

  %minMaxDiff = abs(min(unansweredProbs)-max(unansweredProbs)); 
  minMaxDiff2 = abs(min(unansweredWordProbabilities)-max(unansweredWordProbabilities));
  
  value = minMaxDiff2 / 2;
endfunction    
