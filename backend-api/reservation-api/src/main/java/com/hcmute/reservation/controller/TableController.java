package com.hcmute.reservation.controller;

import com.hcmute.reservation.model.dto.table.AvailableWindowResponse;
import com.hcmute.reservation.model.dto.table.FloorMapTableResponse;
import com.hcmute.reservation.model.dto.table.TableRequest;
import com.hcmute.reservation.model.dto.table.TableResponse;
import com.hcmute.reservation.service.AvailabilityApiService;
import com.hcmute.reservation.service.TableService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.List;

@RestController
@RequestMapping("/api/tables")
@RequiredArgsConstructor
public class TableController {

    private final TableService tableService;
    private final AvailabilityApiService availabilityApiService;

    /** GET /api/tables */
    @GetMapping
    public ResponseEntity<List<TableResponse>> getAllTables() {
        return ResponseEntity.ok(tableService.getAllTables());
    }

    /** POST /api/tables */
    @PostMapping
    public ResponseEntity<TableResponse> createTable(@Valid @RequestBody TableRequest req) {
        return ResponseEntity.status(HttpStatus.CREATED).body(tableService.createTable(req));
    }

    /** PUT /api/tables/{id} */
    @PutMapping("/{id}")
    public ResponseEntity<TableResponse> updateTable(@PathVariable Long id,
                                                     @Valid @RequestBody TableRequest req) {
        return ResponseEntity.ok(tableService.updateTable(id, req));
    }

    /** DELETE /api/tables/{id} */
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> deleteTable(@PathVariable Long id) {
        tableService.deleteTable(id);
        return ResponseEntity.noContent().build();
    }

    /** GET /api/tables/floor-map */
    @GetMapping("/floor-map")
    public ResponseEntity<List<FloorMapTableResponse>> getFloorMap() {
        return ResponseEntity.ok(tableService.getFloorMap());
    }

    /** GET /api/tables/available-windows?guests=&time= */
    @GetMapping("/available-windows")
    public ResponseEntity<List<AvailableWindowResponse>> getAvailableWindows(
            @RequestParam int guests,
            @RequestParam @DateTimeFormat(iso = DateTimeFormat.ISO.DATE_TIME) LocalDateTime time) {
        return ResponseEntity.ok(availabilityApiService.getAvailableWindows(guests, time));
    }

    /** GET /api/tables/reserved-today */
    @GetMapping("/reserved-today")
    public ResponseEntity<List<TableResponse>> getReservedTablesToday() {
        return ResponseEntity.ok(tableService.getReservedTablesToday());
    }
}
