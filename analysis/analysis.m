clear ; close all; clc
pkg load signal;

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
  data = importdata("answers.mat");
  d = data.answers;
  m = data.m;
  d(1) = 1;
else
  m = (max(count) - min(count));
  d = count / m;
  d(1) = 1;
  m = (max(d)-min(d))/2;
end
prompt = 'How many words you would want to answer: ';
wordCount = input(prompt);
value = m / 2;

%positioning figure on display
ss = get(0,'screensize'); %The screen size
width = ss(3);
height = ss(4);
H = figure;
set(H,"visible","on");
vert = 400; %300 vertical pixels
horz = 550; %600 horizontal pixels
%This will place the figure in the top-right corner
set(H,'Position',[width-horz-50, height-vert-100, horz, vert]);

for i = 1:wordCount
  %plotting
  plot (count, d, "o", "markersize", 7);
  hold;
  plot (count, d, 'linewidth', 2);
  plot (count, medfilt1(d, 10), 'linewidth', 3);

  
  fprintf('Looking for closest to %f \n', value);
  [c index] = min(abs(d - value));
  fprintf('The word is "%s"\n', words{index,1});
  
  plot(count(index), d(index),'LineWidth',2, "gx", "markersize", 15)
  legend ("normailzed count","probability locus","median filter", "current question");
  
  %accept user's input
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
%  if index - 2 > 0
%    m_1 = mean(d(2: index-1))
%    upperLimit = index-1;
%    for i= 2:upperLimit
%      if (d(i) ~= 1 && d(i) ~= 0)
%        d(i) = m_1;
%      end  
%    end
%  end  
  
  %modify distibution?

  newMinIndexOfLastKnown = min(find(d ~= 1 & d ~= 0)); 
  newMinIndexOfLastKnown
  d(newMinIndexOfLastKnown)
  d(max(find(d ~= 1 & d ~= 0)))

  newMax = d(newMinIndexOfLastKnown)-min(d);
  value = (newMax)/2;
end  

%saving answers to a file
answers = d;
m = value;
save answers.mat answers m;
