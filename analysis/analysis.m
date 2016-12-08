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

%F = words{:,1};

if exist("answers.mat", 'file') == 2
  d = importdata("answers.mat");
  %d = answers.a;
else
  m = max(count);
  d = count/m;
end

prompt = 'How many words you would want to answer?';
wordCount = input(prompt);

for i = 1:wordCount
  figure(1);
  plot (count, d, "*", "markersize", 7);
  hold;
  plot (count, d);

  %find value closest to 0.5
  %which means uncertainty
  [c index] = min(abs(d-0.5));
  closestValues = d(index) ;
  display(d(index));
  display(words(index));
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