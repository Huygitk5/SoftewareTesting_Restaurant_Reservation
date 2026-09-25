package com.hcmute.reservation.service;

import com.hcmute.reservation.model.dto.table.FloorMapTableResponse;
import com.hcmute.reservation.model.dto.table.TableRequest;
import com.hcmute.reservation.model.dto.table.TableResponse;

import java.util.List;

public interface TableService {
    List<TableResponse> getAllTables();
    TableResponse createTable(TableRequest req);
    TableResponse updateTable(Long id, TableRequest req);
    void deleteTable(Long id);
    List<FloorMapTableResponse> getFloorMap();
    List<TableResponse> getReservedTablesToday();
}