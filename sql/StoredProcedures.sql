-- =========================================
-- Get Voucher Data Stored Procedure
-- =========================================
CREATE PROCEDURE [dbo].[GetVoucherData]
    @ActionType INT,
    @EnvelopeType INT,
    @BatchNumber INT,
    @FamilyCode BIGINT = NULL,
    @ClosureNumber INT = NULL,
    @VoucherGroup BIGINT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Update uniqnum for combined envelopes if needed
    IF @ActionType = 3
    BEGIN
        -- Update for property-level vouchers
        UPDATE shovarhead 
        SET uniqnum = -100
        FROM shovarhead 
        INNER JOIN shovarhead AS shovarhead_1 
            ON shovarhead.hskod = shovarhead_1.hskod 
            AND shovarhead.mspkod = shovarhead_1.mspkod 
            AND shovarhead.mnt = shovarhead_1.mnt
        WHERE shovarhead.mnt = @BatchNumber 
            AND ISNULL(shovarhead.shnati, 0) = 0 
            AND ISNULL(shovarhead.shovarmsp, 0) = 0
            AND ISNULL(shovarhead_1.shnati, 0) = 0 
            AND ISNULL(shovarhead_1.shovarmsp, 0) = 0
            AND (ISNULL(shovarhead.manahovnum, 0) = 0) 
            AND (ISNULL(shovarhead_1.manahovnum, 0) <> 0)
            AND (@FamilyCode IS NULL OR shovarhead.mspkod = @FamilyCode)
            AND (@ClosureNumber IS NULL OR shovarhead.sgrnum = @ClosureNumber)
            AND (@VoucherGroup IS NULL OR shovarhead.kvuzashovar = @VoucherGroup);

        -- Update for family-level vouchers
        UPDATE shovarhead 
        SET uniqnum = -100
        FROM shovarhead 
        INNER JOIN shovarhead AS shovarhead_1 
            ON shovarhead.mspkod = shovarhead_1.mspkod 
            AND shovarhead.mnt = shovarhead_1.mnt
        WHERE shovarhead.mnt = @BatchNumber 
            AND ISNULL(shovarhead.shnati, 0) = 0 
            AND ISNULL(shovarhead.shovarmsp, 0) = 1
            AND ISNULL(shovarhead_1.shnati, 0) = 0 
            AND ISNULL(shovarhead_1.shovarmsp, 0) = 1
            AND (ISNULL(shovarhead.manahovnum, 0) = 0) 
            AND (ISNULL(shovarhead_1.manahovnum, 0) <> 0)
            AND (@FamilyCode IS NULL OR shovarhead.mspkod = @FamilyCode)
            AND (@ClosureNumber IS NULL OR shovarhead.sgrnum = @ClosureNumber)
            AND (@VoucherGroup IS NULL OR shovarhead.kvuzashovar = @VoucherGroup);
    END

    -- Select voucher data
    SELECT 
        sh.mspkod,
        sh.manahovnum,
        sl.mtfnum,
        sh.shovarmsp,
        IIF(ISNULL(sh.shovarmsp, 0) = 0, sh.hskod, '0') AS miun,
        sh.uniqnum,
        sh.shnati,
        -- Additional dynamic fields will be added based on envelope structure
        sh.*,
        sl.*,
        shn.*,
        shd.*
    FROM shovarhead sh
    INNER JOIN shovarlines sl ON sh.shovar = sl.shovar
    LEFT JOIN shovarheadnx shn ON sh.shovar = shn.shovar
    LEFT JOIN shovarheadDynamic shd ON sh.shovar = shd.shovar
    WHERE sh.mnt = @BatchNumber
        AND (sndto < CASE WHEN ISNULL((SELECT PrintEmailMtf FROM param3), 0) = 0 THEN 3 ELSE 4 END OR shnati <> 0)
        AND (@FamilyCode IS NULL OR sh.mspkod = @FamilyCode)
        AND (@ClosureNumber IS NULL OR sh.sgrnum = @ClosureNumber)
        AND (@VoucherGroup IS NULL OR sh.kvuzashovar = @VoucherGroup)
    ORDER BY 
        sh.nameinsvr,
        sh.mspkod,
        IIF(ISNULL(sh.shovarmsp, 0) = 0, sh.hskod, '0'),
        sl.mtfnum,
        sh.manahovnum;
END
GO

-- =========================================
-- Get Envelope Structure Stored Procedure
-- =========================================
CREATE PROCEDURE [dbo].[GetEnvelopeStructure]
    @EnvelopeType INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Get main structure
    SELECT 
        m.inname,
        m.fldtype,
        m.length,
        m.realseder,
        m.recordset
    FROM mivnemtf m
    WHERE 
        m.sugmtf = @EnvelopeType
        AND ISNULL(m.show, 0) = 1 
        AND ISNULL(m.notInMtf, 0) = 0
    ORDER BY m.realseder;

    -- Get header parameters
    SELECT 
        h.dosheb,
        h.numOfDigits,
        h.positionOfShnati,
        h.numOfPerutLines
    FROM mivnemtfhead h
    WHERE h.kodsugmtf = @EnvelopeType;
END
GO

-- =========================================
-- Get System Parameters Stored Procedure
-- =========================================
CREATE PROCEDURE [dbo].[GetSystemParams]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        revheb,
        -- Additional system parameters as needed
        PrintEmailMtf
    FROM paramset
    CROSS JOIN param3;
END
GO