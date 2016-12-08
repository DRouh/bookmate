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
  m = (max(count) - min(count));
  d = count / m;
end


%positioning figure on display
ss = get(0,'screensize'); %The screen size
width = ss(3);
height = ss(4);
H = figure;
vert = 400; %300 vertical pixels
horz = 550; %600 horizontal pixels
%This will place the figure in the top-right corner
set(H,'Position',[width-horz-50, height-vert-100, horz, vert]);


prompt = 'How many words you would want to answer: ';
wordCount = input(prompt);
value = (m)/2;
for i = 1:wordCount
  %plotting
  plot (count, d, "*", "markersize", 7);
  hold;
  plot (count, d);

  %find value closest to 0.5
  %which means uncertainty
  fprintf('Looking for closest to %f \n', value);
  [c index] = min(abs(d - value));
  fprintf('The word is "%s"\n', words{index,1});
  userAsnwered = false;
  
  while ~userAsnwered  
    prompt = 'Do you know this word (1/0)? ';
    x = input(prompt);

    if (x == 1) 
      userAsnwered = true;
      d(index) = 1;   
    elseif (x == 0)
      userAsnwered = true;
      d(index) = 0;
    end
    
    if i ~= wordCount
      clf;
    end  
  end 
	
  %todo keep track of 0s and 1s because they mean a Certain answer to the question!
  if index - 2 > 0
    m_1 = mean(d(2: index-1))
    upperLimit = index-1;
    for i= 2:upperLimit
      d(i) = m_1;
    end
  end  
  


  %rebuild distibution?
  newMinIndexOfLastKnown = min(find(d ~= 1)); 
  newMax = max(d(newMinIndexOfLastKnown))-min(d);
  value = (newMax)/2;
end  

%saving answers to a file
answers = d;
save answers.mat answers;