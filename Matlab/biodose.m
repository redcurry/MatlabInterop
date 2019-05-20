function y = biodose(dose, n, alphaBeta)
    y = arrayfun(@eqd2, dose);
    
    function y = eqd2(d)
        y = d * (d / n + alphaBeta) / (2 + alphaBeta);
    end
end