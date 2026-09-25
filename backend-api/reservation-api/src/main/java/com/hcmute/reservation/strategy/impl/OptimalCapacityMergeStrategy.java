package com.hcmute.reservation.strategy.impl;

import com.hcmute.reservation.model.entity.TableInfo;
import com.hcmute.reservation.strategy.TableAllocationStrategy;
import org.springframework.core.annotation.Order;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.List;
import java.util.stream.Collectors;

@Component
@Order(2)
public class OptimalCapacityMergeStrategy implements TableAllocationStrategy {

    @Override
    public List<TableInfo> allocate(int guestCount, List<TableInfo> freeTables) {
        if (freeTables.size() < 2) return Collections.emptyList();


        List<TableInfo> sortedCandidates = freeTables.stream()
                .sorted(Comparator.comparingInt(TableInfo::getCapacity)
                        .thenComparing(TableInfo::getTableId))
                .collect(Collectors.toList());

        int maxCapacity = sortedCandidates.stream().mapToInt(TableInfo::getCapacity).sum();
        if (maxCapacity < guestCount) return Collections.emptyList();

        List<List<TableInfo>> bestByCapacity = new ArrayList<>(Collections.nCopies(maxCapacity + 1, null));
        bestByCapacity.set(0, List.of());

        for (TableInfo table : sortedCandidates) {
            for (int current = maxCapacity - table.getCapacity(); current >= 0; current--) {
                List<TableInfo> existing = bestByCapacity.get(current);
                if (existing == null) continue;

                int nextCapacity = current + table.getCapacity();
                List<TableInfo> candidateCombo = new ArrayList<>(existing);
                candidateCombo.add(table);

                List<TableInfo> stored = bestByCapacity.get(nextCapacity);
                if (stored == null || isPreferredForSameCapacity(candidateCombo, stored)) {
                    bestByCapacity.set(nextCapacity, List.copyOf(candidateCombo));
                }
            }
        }

        List<TableInfo> best = null;
        for (int capacity = guestCount; capacity <= maxCapacity; capacity++) {
            List<TableInfo> combo = bestByCapacity.get(capacity);
            if (combo == null || combo.size() < 2) continue;

            if (best == null || isBetterMergeResult(combo, best))
                best = combo;
        }

        return best == null ? Collections.emptyList() : best;
    }

    @Override
    public int getOrder() { return 2; }

    private boolean isPreferredForSameCapacity(List<TableInfo> candidate, List<TableInfo> existing) {
        if (candidate.size() != existing.size()) return candidate.size() < existing.size();
        return compareIdSequences(candidate, existing) < 0;
    }

    private boolean isBetterMergeResult(List<TableInfo> candidate, List<TableInfo> currentBest) {
        int candidateCapacity = candidate.stream().mapToInt(TableInfo::getCapacity).sum();
        int bestCapacity = currentBest.stream().mapToInt(TableInfo::getCapacity).sum();
        if (candidateCapacity != bestCapacity) return candidateCapacity < bestCapacity;
        if (candidate.size() != currentBest.size()) return candidate.size() < currentBest.size();
        return compareIdSequences(candidate, currentBest) < 0;
    }

    private int compareIdSequences(List<TableInfo> first, List<TableInfo> second) {
        List<Long> firstIds = first.stream().map(TableInfo::getTableId).sorted().collect(Collectors.toList());
        List<Long> secondIds = second.stream().map(TableInfo::getTableId).sorted().collect(Collectors.toList());
        for (int i = 0; i < Math.min(firstIds.size(), secondIds.size()); i++) {
            int cmp = firstIds.get(i).compareTo(secondIds.get(i));
            if (cmp != 0) return cmp;
        }
        return Integer.compare(firstIds.size(), secondIds.size());
    }
}