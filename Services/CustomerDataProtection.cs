using System.Text.RegularExpressions;
using CNPM.Models;
using Microsoft.EntityFrameworkCore;

namespace CNPM.Services;

public static class CustomerDataProtection
{
    private static readonly Dictionary<char, char> EncryptPhoneMap = new()
    {
        ['0'] = 'Q', ['1'] = 'W', ['2'] = 'E', ['3'] = 'R', ['4'] = 'T',
        ['5'] = 'Y', ['6'] = 'U', ['7'] = 'I', ['8'] = 'O', ['9'] = 'P'
    };

    private static readonly Dictionary<char, char> DecryptPhoneMap = EncryptPhoneMap
        .ToDictionary(x => x.Value, x => x.Key);

    private static readonly Regex PlainPhoneRegex = new("^0\\d{9}$", RegexOptions.Compiled);

    public static string? EncryptName(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var chars = value.Select(c => (char)(c + 3)).ToArray();
        return new string(chars);
    }

    public static string? DecryptName(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var chars = value.Select(c => (char)(c - 3)).ToArray();
        return new string(chars);
    }

    public static string? EncryptPhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        if (!IsPlainPhone(value))
        {
            return value;
        }

        return new string(value.Select(c => EncryptPhoneMap[c]).ToArray());
    }

    public static string? DecryptPhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        if (IsPlainPhone(value))
        {
            return value;
        }

        if (!value.All(c => DecryptPhoneMap.ContainsKey(c)))
        {
            return value;
        }

        return new string(value.Select(c => DecryptPhoneMap[c]).ToArray());
    }

    public static bool IsPlainPhone(string? value)
    {
        return !string.IsNullOrWhiteSpace(value) && PlainPhoneRegex.IsMatch(value);
    }

    public static bool IsEncryptedPhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return !IsPlainPhone(value) && value.All(c => DecryptPhoneMap.ContainsKey(c));
    }

    public static void EncryptCustomer(TblKhachHang khachHang)
    {
        if (khachHang == null)
        {
            return;
        }

        if (IsPlainPhone(khachHang.SSdt))
        {
            khachHang.STenKh = EncryptName(khachHang.STenKh);
            khachHang.SSdt = EncryptPhone(khachHang.SSdt);
            khachHang.SDiaChi = EncryptName(khachHang.SDiaChi);
        }
    }

    public static void DecryptCustomer(TblKhachHang khachHang)
    {
        if (khachHang == null)
        {
            return;
        }

        if (IsEncryptedPhone(khachHang.SSdt))
        {
            khachHang.STenKh = DecryptName(khachHang.STenKh);
            khachHang.SSdt = DecryptPhone(khachHang.SSdt);
            khachHang.SDiaChi = DecryptName(khachHang.SDiaChi);
        }
    }

    public static async Task<int> EnsureExistingDataEncryptedAsync(PharmacyDbContext context, CancellationToken cancellationToken = default)
    {
        var customers = await context.TblKhachHangs
            .Where(x => x.SSdt != null)
            .ToListAsync(cancellationToken);

        var updated = 0;
        foreach (var customer in customers)
        {
            if (!IsPlainPhone(customer.SSdt))
            {
                continue;
            }

            EncryptCustomer(customer);
            updated++;
        }

        if (updated > 0)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        return updated;
    }
}
