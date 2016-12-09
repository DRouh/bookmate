function [words, count, p, v] = loadData(dataFileName, textFileName)
  
  %if we already have converted txt file to mat file then load it
  if exist(dataFileName, 'file') == 2
    fprintf('Loading data....\n');
    data = importdata(dataFileName);
    words = data.w;
    count = data.c;
  %read text file in the other case
  else
    [w, c] = textread(textFileName, '%s %f', 'delimiter' , ' ', 2);
    save data.mat w c
    words = w;
    count = c;
  end

  %if there's already answers saved then load them 
  if exist("answers.mat", 'file') == 2
    data = importdata("answers.mat");
    p = data.answers;
    v = data.v;
    p(1) = 1; %because octave calcs it tob be 1.01
    p(end) = 0;
  else
    %m - is the point (word) that we want answer for
    v = (max(count) - min(count));
    %normalize counts to reflet probability
    p = count / v;
    p(1) = 1;%because octave calcs it tob be 1.01
    p(end) = 0;
    v = (max(p)-min(p))/2;
  end
endfunction