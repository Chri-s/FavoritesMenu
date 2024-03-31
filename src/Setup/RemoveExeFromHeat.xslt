<?xml version="1.0" ?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://wixtoolset.org/schemas/v4/wxs">

	<!-- Copy all attributes and elements to the output. -->
	<xsl:template match="@*|*">
		<xsl:copy>
			<xsl:apply-templates select="@*" />
			<xsl:apply-templates select="*" />
		</xsl:copy>
	</xsl:template>

	<xsl:output method="xml" indent="yes" />

	<!-- Exclude exe files from output -->
	<xsl:key name="exe-search" match="wix:Component[substring(wix:File/@Source, string-length(wix:File/@Source) - string-length('FavoritesMenu.exe') + 1) = 'FavoritesMenu.exe']" use="@Id" />
	<xsl:template match="wix:Component[key('exe-search', @Id)]" />
	<xsl:template match="wix:ComponentRef[key('exe-search', @Id)]" />
</xsl:stylesheet>