﻿using System.Xml.Serialization;

namespace TableCloth2.Shared.Models.Catalog;

/// <summary>
/// 카탈로그 상의 인터넷 서비스 분류를 나타냅니다.
/// </summary>
[Serializable, XmlType]
public enum CatalogInternetServiceCategory : short
{
    /// <summary>
    /// 기타
    /// </summary>
    [XmlEnum(nameof(Other))]
    [EnumDisplayOrder(Order = 8)]
    Other = 0,

    /// <summary>
    /// 인터넷 뱅킹
    /// </summary>
    [XmlEnum(nameof(Banking))]
    [EnumDisplayOrder(Order = 1)]
    Banking,

    /// <summary>
    /// 금융
    /// </summary>
    [XmlEnum(nameof(Financing))]
    [EnumDisplayOrder(Order = 2)]
    Financing,

    /// <summary>
    /// 투자
    /// </summary>
    [XmlEnum(nameof(Security))]
    [EnumDisplayOrder(Order = 3)]
    Security,

    /// <summary>
    /// 보험
    /// </summary>
    [XmlEnum(nameof(Insurance))]
    [EnumDisplayOrder(Order = 4)]
    Insurance,

    /// <summary>
    /// 신용 카드
    /// </summary>
    [XmlEnum(nameof(CreditCard))]
    [EnumDisplayOrder(Order = 5)]
    CreditCard,

    /// <summary>
    /// 정부, 공공기관
    /// </summary>
    [XmlEnum(nameof(Government))]
    [EnumDisplayOrder(Order = 6)]
    Government,

    /// <summary>
    /// 교육
    /// </summary>
    [XmlEnum(nameof(Education))]
    [EnumDisplayOrder(Order = 7)]
    Education,
}
