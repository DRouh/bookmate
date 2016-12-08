clear ; close all; clc

if exist("data.mat", 'file') == 2
  fprintf('Loading data....\n');
  data = importdata("data.mat");
  words = data.w;
  count = data.c;
else
  [w, c] = textread('short.txt', '%s %f', 'delimiter' , ' ', 2);
  save data.mat w c
  words = w;
  count = c;
end

if exist("answers.mat", 'file') == 2
  d = importdata("answers.mat");
else
  m = (max(count) - min(count)) / 2;
  d = count / m;
end

prompt = 'How many words you would want to answer: ';
wordCount = input(prompt);

for i = 1:wordCount
  %plotting
  figure(1);
  plot (count, d, "*", "markersize", 7);
  hold;
  plot (count, d);

  %find value closest to 0.5
  %which means uncertainty
  value = (max(d) - min(d))/2;
  fprintf('Looking for closest to %f \n', value);
  
  [c index] = min(abs(d - value));
  
  fprintf('The word is "%s"\n', words{index,1});
  userAsnwered = false;
  
  while ~userAsnwered  
    prompt = 'Do you know this word (1/0)? ';
    x = input(prompt);

    if (x = 1) 
      userAsnwered = true;
      d(index) = 1;
      
    elseif (x = 0)
      userAsnwered = true;
      d(index) = 0;
      
    else 
      userAsnwered = false;
    end
    
    if i ~= wordCount
      clf;
    end  
  end 
  %rebuild distibution?
end  

%saving answers to a file
answers = d;
save answers.mat answers;