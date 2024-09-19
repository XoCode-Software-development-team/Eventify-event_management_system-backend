using eventify_backend.DTOs;
using eventify_backend.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class GeocodingService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GeocodingService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<List<GeocodedLocationDTO>> GeocodeServicesAsync(List<Service> services)
    {
        List<GeocodedLocationDTO> geocodedLocations = new List<GeocodedLocationDTO>();

        foreach (var service in services)
        {
            foreach (var location in service.VendorSRLocations!)
            {
                try
                {
                    var apiKey = _configuration["GoogleMaps:ApiKey"];
                    //var address =$"{location!.District},${location!.Country}";
                    var address = $"{location!.HouseNo}, {location?.Area}, {location!.District}, {location?.State}, {location!.Country}";
                    var encodedAddress = Uri.EscapeDataString(address);
                    var apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

                    var response = await _httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<GoogleGeocodingResponse>(json);

                        if (result!.Status == "OK" && result.Results!.Any())
                        {
                            var locationData = result.Results!.First().Geometry?.Location;
                            geocodedLocations.Add(new GeocodedLocationDTO
                            {
                                Latitude = locationData!.Lat,
                                Longitude = locationData.Lng,
                                SoRId = service.SoRId,
                                District = location.District!
                            });
                        }
                        else
                        {
                            throw new Exception($"Geocoding API error: {result.Status}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to geocode address: {address}. StatusCode: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error geocoding address for service {service.SoRId}: {ex.Message}", ex);
                }
            }
        }

        return geocodedLocations;
    }

    public async Task<List<GeocodedLocationDTO>> GeocodeResourcesAsync(List<Resource> resources)
    {
        List<GeocodedLocationDTO> geocodedLocations = new List<GeocodedLocationDTO>();

        foreach (var resource in resources)
        {
            foreach (var location in resource.VendorSRLocations!)
            {
                try
                {
                    var apiKey = _configuration["GoogleMaps:ApiKey"];
                    //var address =$"{location!.District},${location!.Country}";
                    var address = $"{location!.HouseNo}, {location?.Area}, {location!.District}, {location?.State}, {location!.Country}";
                    var encodedAddress = Uri.EscapeDataString(address);
                    var apiUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={encodedAddress}&key={apiKey}";

                    var response = await _httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<GoogleGeocodingResponse>(json);

                        if (result!.Status == "OK" && result.Results!.Any())
                        {
                            var locationData = result.Results!.First().Geometry?.Location;
                            geocodedLocations.Add(new GeocodedLocationDTO
                            {
                                Latitude = locationData!.Lat,
                                Longitude = locationData.Lng,
                                SoRId = resource.SoRId,
                                District = location.District!
                            });
                        }
                        else
                        {
                            throw new Exception($"Geocoding API error: {result.Status}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Failed to geocode address: {address}. StatusCode: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error geocoding address for service {resource.SoRId}: {ex.Message}", ex);
                }
            }
        }

        return geocodedLocations;
    }

}
