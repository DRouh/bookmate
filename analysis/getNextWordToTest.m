function value = getNextWordToTest(p)
  %min - for the words with the greatest count
  %max - for the words with the smallest count
  
  
  %find poin near which we seek next word to ask
  %!!!if the word was true - go right, if not then go left
  notTestedWords = p(find(p ~= 1 && p ~= 0))
  meanP = mean(notTestedWords)
  unansweredWordProbabilities = find(p ~= 1 && p ~= 0 && meanP > p); 
  unansweredProbs = p(unansweredWordProbabilities);
  
  minMaxDiff = abs(min(unansweredProbs)-max(unansweredProbs)); 
  minMaxDiff2 = abs(mode(unansweredProbs)-max(unansweredProbs));
  
  value = minMaxDiff2 / 2;
endfunction    