clear ; close all; clc
pkg load signal;  
#pkg load splines;

%loading data (words and count), vector of probability (d) and m (mean value to consider)
[words,count, p, v] = loadData("data.mat", "short.txt");

prompt = 'How many words you would want to answer: ';
wordCount = input(prompt);
value = v / 2;

%positioning figure on display
H = prepareFigure();

for i = 1:wordCount
  fprintf('Looking for closest to %f \n', value);
  [c index] = min(abs(p - value));
  fprintf('The word is "%s"\n', words{index, 1});
  
  %plotting
  plotData(count, p, value, index)
 
  %accept user's input
  userAsnwered = false;
  while ~userAsnwered  
    prompt = 'Do you know this word (1/0)? ';
    x = input(prompt);

    if (x == 1) 
      userAsnwered = true;
      p(index) = 1;   
    elseif (x == 0)
      userAsnwered = true;
      p(index) = 0;
    end
  end 
	
  %min - for the words with the greatest count
  %max - for the words with the smallest count
  
  %find poin near which we seek next word to ask
  unansweredWordProbabilities = find(p ~= 1 & p ~= 0); 
  unansweredProbs = p(unansweredWordProbabilities);
  
  minMaxDiff = abs(min(unansweredProbs)-max(unansweredProbs)); 
  minMaxDiff2 = abs(mode(unansweredProbs)-max(unansweredProbs));
  
  %value = mode(p(unansweredWordProbabilities));
  value = minMaxDiff / 2;
end  

%saving answers to a file
answers = p;
v = value;
save answers.mat answers v;

%todo
%1. remove median filter at all
%2. spline or polynom of 2 pow over points that were answered

