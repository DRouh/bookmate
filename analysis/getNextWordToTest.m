function value = getNextWordToTest(p, prevIndex)
  %if user knew previous word - test more difficult words; otherwise - easier
  if prevIndex < length(p)
    if p(prevIndex) == 1
      right = p(prevIndex:end);
      notTestedWords = right(find(right ~= 1 & right ~= 0));
      unansweredWordProbabilities = notTestedWords;
    else
      left = p(1:prevIndex);
      notTestedWords = left(find(left ~= 1 & left ~= 0));
      unansweredWordProbabilities = notTestedWords;
    end
  else
    notTestedWords = p(find(p ~= 1 & p ~= 0));
    meanOfNotTestedP = mean(notTestedWords);
    unansweredWordProbabilities = notTestedWords(find(meanOfNotTestedP > notTestedWords)); 
  end  

 
  halfWayIndex = floor(length(unansweredWordProbabilities) / 2);
  %fix for edge cases. get to random unanswered point if got beyound limits
  if halfWayIndex == 0
        fprintf('Loading data....\n');
    unansweredWordProbabilities = p(find(p ~= 1 & p ~= 0));
    rndIndex = round(rand(1)*length(unansweredWordProbabilities))
    value = unansweredWordProbabilities(rndIndex);
  else  
    value = unansweredWordProbabilities(halfWayIndex);
  end
  
endfunction    
