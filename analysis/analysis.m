clear ; close all; clc
pkg load signal;  

%loading data (words and count), vector of probability (d) and m (mean value to consider)
[words,count, p, v] = loadData('data.mat', 'short.txt');

wordCount = input('How many words you would want to be shown: ');
value = v / 2;

%positioning figure on display
H = prepareFigure();
for i = 1:wordCount
  if p(end) == 0 || p(end) == 1 
    %determine where it's best to pick new word to test
    value = getNextWordToTest(p);
    [c index] = findClosest(p, value);
  else
    index = length(p);
    value = p(end);
  end
  
  %plotting
  plotData(count, p, value, index, value);

  %accept user's input
  word = words{index, 1};
  p(index) = askUser(word, i, wordCount);
end  

%saving answers to a file
answers = p;
v = value;
save answers.mat answers v;