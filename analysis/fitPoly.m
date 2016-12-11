function polynY = fitPoly(maxXValue, x, y, degree)
    %fitting polynomial
    %1. naive scaling of y's to help avoid numerical errors
    a = 1;
    b = maxXValue;
    y_scaled = (y-min(y))*(b-a)/(max(y)-min(y)) + a;
    polyn = polyfit(x, y, degree);
    polynY = polyval(polyn, x);
    
    %2. lilmit polynomial to lie in [0;1] range
    polynY(polynY<0) = 0;
    polynY(polynY>1) = 1;
endfunction  