function H = prepareFigure()
  ss = get(0,'screensize'); %The screen size
  width = ss(3);
  height = ss(4);
  H = figure;
  set(H,"visible","on");
  vert = 400; %300 vertical pixels
  horz = 550; %600 horizontal pixels
  %This will place the figure in the top-right corner
  set(H,'Position',[width-horz-50, height-vert-100, horz, vert]);
endfunction