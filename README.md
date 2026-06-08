# Calculations

This app features

- Graffana for viewing logs, metrics and tracing
- Simple extensibilty for adding new calculations
- Supports dynamically set request and response bodies
- FE dynamically creates required fields for the selected calculation based on generated meta data sent from backend
- Generates meta data on startup improve performance

## Example requests

**Run a calculation**

```
curl -X POST https://localhost:8081/calculations/combinedwith -H "Content-Type: application/json" -d "{ \"probabilityA\": 0.5, \"probabilityB\": 0.5 }"
```

Returns 
```
{"value":0.25}
```

**Get all meta data about calculations**

```
curl https://localhost:8081/calculations
```
Returns

```
[
    {
        "name":"CombinedWith",
        "requestFields":[
            {
                "name":"ProbabilityA",
                "type":"number"
            },
            {
                "name":"ProbabilityB",
                "type":"number"
            }
        ],
        "responseFields":[
            {
                "name":"Value",
                "type":"number"
            }
        ]
    },
    {
        "name":"Either",
        "requestFields":[
            {
                "name":"ProbabilityA",
                "type":"number"
            },
            {
                "name":"ProbabilityB",
                "type":"number"
            }
        ],
        "responseFields":[
            {
                "name":"Value",
                "type":"number"
            }
        ]
    }

]
```

## Running

### prerequits

- Docker desktop
- Visual Studio 2026
- node

Open slnx file in visual studio and press F5 or click Debug -> Start Debugging to start the application

## Obeserveability

I setup the code to use OTEL endpoints so the telemetry collector is decoupled from the implementation. Any tool that support OTEL endpoints could be swapped in. To keep it simple I have used grafanas lgtm image that groups various observability tools together to capture logs, metrics and traces.

The https://github.com/grafana/docker-otel-lgtm image provides

- OpenTelemetry Collector
- Prometheus (metrics)
- Tempo (traces)
- Loki (logs)
- Pyroscope (profiles) 
- Grafana (Dashboards)