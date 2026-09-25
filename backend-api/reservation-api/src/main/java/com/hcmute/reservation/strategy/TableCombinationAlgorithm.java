package com.hcmute.reservation.strategy;

import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.service.ConfigProviderService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

@Component
@RequiredArgsConstructor
public class TableCombinationAlgorithm {

    private final ConfigProviderService configProvider;

    // Trả về tổ hợp 1 mảng bàn tốt nhất
    public List<TableInfo> findBestTableCombination(List<TableInfo> availableTables, int targetGuests) {
        List<TableInfo> sortedTables = availableTables.stream()
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).reversed())
                .collect(Collectors.toList());

        List<TableInfo> bestCombination = new ArrayList<>();
        int[] bestDiff = { Integer.MAX_VALUE };
        int[] minTables = { Integer.MAX_VALUE };

        backtrack(sortedTables, targetGuests, 0, new ArrayList<>(), 0, bestCombination, bestDiff, minTables);
        return bestCombination;
    }

    // Trả về danh sách TẤT CẢ các tổ hợp ghép bàn có thể (Dùng cho Walk-in Options)
    public List<List<TableInfo>> findWalkInOptionCombinations(List<TableInfo> availableTables, int targetGuests) {
        List<TableInfo> sortedTables = availableTables.stream()
                .sorted(Comparator.comparingInt(TableInfo::getCapacity).reversed().thenComparing(TableInfo::getTableId))
                .collect(Collectors.toList());

        List<List<TableInfo>> combinations = new ArrayList<>();
        backtrackWalkInOptionCombinations(sortedTables, targetGuests, 0, new ArrayList<>(), 0, combinations);
        return combinations;
    }

    private void backtrack(List<TableInfo> tables, int target, int start,
                           List<TableInfo> currentCombo, int currentSum,
                           List<TableInfo> bestCombo, int[] bestDiff, int[] minTables) {
        int maxMergeTables = configProvider.getMaxMergeTables();
        int maxCapacityOverflow = configProvider.getMaxCapacityOverflow();
        if (currentSum >= target) {
            int diff = currentSum - target;
            if (diff <= maxCapacityOverflow) {
                if (diff < bestDiff[0] || (diff == bestDiff[0] && currentCombo.size() < minTables[0])) {
                    bestDiff[0] = diff;
                    minTables[0] = currentCombo.size();
                    bestCombo.clear();
                    bestCombo.addAll(currentCombo);
                }
            }
            return;
        }
        if (currentCombo.size() > maxMergeTables) return;

        for (int i = start; i < tables.size(); i++) {
            currentCombo.add(tables.get(i));
            backtrack(tables, target, i + 1, currentCombo, currentSum + tables.get(i).getCapacity(), bestCombo, bestDiff, minTables);
            currentCombo.remove(currentCombo.size() - 1);
        }
    }

    private void backtrackWalkInOptionCombinations(List<TableInfo> tables, int target, int start,
                                                   List<TableInfo> currentCombo, int currentSum,
                                                   List<List<TableInfo>> combinations) {
        int maxCapacityOverflow = configProvider.getMaxCapacityOverflow();
        if (combinations.size() >= 20) return;
        if (currentSum >= target) {
            int diff = currentSum - target;
            if (diff <= maxCapacityOverflow) combinations.add(new ArrayList<>(currentCombo));
            return;
        }
        if (currentCombo.size() >= 4) return;

        for (int i = start; i < tables.size(); i++) {
            currentCombo.add(tables.get(i));
            backtrackWalkInOptionCombinations(tables, target, i + 1, currentCombo, currentSum + tables.get(i).getCapacity(), combinations);
            currentCombo.remove(currentCombo.size() - 1);
        }
    }
}
